using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Leap;

namespace com.hive.projectr
{
    public class InputManager : SingletonBase<InputManager>, ICoreManager
    {
        public Vector3 CursorScreenPosition
        {
            get
            {
                if (UICursor != null)
                {
                    return UICursor.GetPosition();
                }

                return Vector3.zero;
            }
        }

        public MotionTracker MotionTracker
        {
            get
            {
                if (_motionTracker == null)
                {
                    _motionTracker = GameObject.FindGameObjectWithTag(TagNames.MotionTracker).GetComponent<MotionTracker>();
                }
                return _motionTracker;
            }
        }
        private MotionTracker _motionTracker;

        public UICursor UICursor
        {
            get
            {
                if (_uiCursor == null)
                {
                    _uiCursor = GameObject.FindGameObjectWithTag(TagNames.Cursor).GetComponent<UICursor>();
                }
                return _uiCursor;
            }
        }
        private UICursor _uiCursor;
        private Vector3 _cursorPos;
        private Vector3 _initCursorPos;
        private bool _isCursorInitialized;
        private PointerEventData _pointerEventData;
        private EventSystem _eventSystem;
        private GameObject _currentTarget;
        private GameObject _currentClickable;
        private HashSet<GameObject> _currentPressing = new HashSet<GameObject>();
        private HashSet<GameObject> _currentHovering = new HashSet<GameObject>();
        private Hand _currentHand;

        private bool _isPinching;

        #region Lifecycle
        public void OnInit()
        {
            _eventSystem = EventSystem.current;
            _pointerEventData = new PointerEventData(_eventSystem);

            MonoBehaviourUtil.OnUpdate += Tick;

            Cursor.visible = false;
        }

        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }

        private void Tick()
        {
            CursorTick();
        }

        private void CursorTick()
        {
            if (UICursor != null && MotionTracker != null)
            {
                _currentHand = null;

                var position = Vector3.zero;

                if (MotionTracker != null)
                {
                    var handModelManager = MotionTracker.HandModelManager;
                    if (handModelManager != null)
                    {
                        if (handModelManager.HandModelPairs != null && handModelManager.HandModelPairs.Count > 0)
                        {
                            var handModelPair = handModelManager.HandModelPairs[0];
                            var rightHand = handModelPair.Right.GetLeapHand();
                            if (rightHand != null)
                            {
                                if (HasValidHandRelativeWorldPosition(rightHand, out position))
                                {
                                    _currentHand = rightHand;
                                }
                            }
                            else
                            {
                                var leftHand = handModelPair.Left.GetLeapHand();
                                if (HasValidHandRelativeWorldPosition(leftHand, out position))
                                {
                                    _currentHand = leftHand;
                                }
                            }
                        }
                    }
                }

                if (_currentHand != null)
                {
                    var cursorPos = position * MotionTracker.Sensitivity * MotionTracker.BaseSensitivityScale;

                    if (!_isCursorInitialized)
                    {
                        _isCursorInitialized = true;
                        _initCursorPos = cursorPos - new Vector3(Screen.width / 2, 0);
                    }

                    _cursorPos = cursorPos - _initCursorPos;

                    UICursor.SetPosition(_cursorPos);

                    CursorInputTick();
                }
            }
        }

        private void CursorInputTick()
        {
            _pointerEventData.position = UICursor.GetPosition();

            // do raycasts and find all objects beneath the cursor
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            _eventSystem.RaycastAll(_pointerEventData, raycastResults);

            GameObject newTarget = raycastResults.Count > 0 ? raycastResults[0].gameObject : null;
            if (newTarget != _currentTarget)
            {
                if (_currentTarget != null)
                {
                    TryTriggerPointerExit(_currentTarget, _pointerEventData);
                }

                if (newTarget != null)
                {
                    TryTriggerPointerEnter(newTarget, _pointerEventData);
                }

                _currentTarget = newTarget;
            }

            var newClickable = GetPointerClickableObject(raycastResults);
            if (newClickable != _currentClickable)
            {
                if (_currentClickable != null)
                {
                    TryTriggerPointerExit(_currentClickable, _pointerEventData);
                }

                if (newTarget != null)
                {
                    TryTriggerPointerEnter(newTarget, _pointerEventData);
                }

                _currentClickable = newClickable;
            }

            // clear clicking
            var currentClicking = new HashSet<GameObject>();

            if (_currentHand != null && _currentHand.Fingers.Count >= 2)
            {
                var thumb = _currentHand.Fingers[0];
                var index = _currentHand.Fingers[1];
                var isPinching = IsPinching();
                if (isPinching)
                {
                    if (!_isPinching) // pointer down
                    {
                        OnPointerDown();
                    }
                }
                else
                {
                    if (_isPinching) // pointer up
                    {
                        OnPointerUp();
                    }
                }

                _isPinching = isPinching;
            }
            else
            {
                if (GetKeyDown(KeyCode.Space))
                {
                    OnPointerDown();
                }

                if (GetKeyUp(KeyCode.Space))
                {
                    OnPointerUp();
                }
            }

            void OnPointerDown()
            {
                _pointerEventData.pressPosition = _pointerEventData.position;
                _pointerEventData.pointerPressRaycast = raycastResults[0];

                var pressable = GetPointerPressableObject(raycastResults);
                if (pressable != null)
                {
                    TryTriggerPointerDown(pressable, _pointerEventData);
                }
            }

            void OnPointerUp()
            {
                var currentPressing = new HashSet<GameObject>(_currentPressing);

                foreach (var obj in currentPressing)
                {
                    if (obj.GetComponent<IPointerUpHandler>() != null)
                    {
                        TryTriggerPointerUp(obj, _pointerEventData);
                    }
                    if (obj.GetComponent<IPointerClickHandler>() != null)
                    {
                        if (_currentHovering.Contains(obj))
                        {
                            TryTriggerPointerClick(obj, _pointerEventData);
                        }
                    }
                }

                _currentPressing.Clear();
                _pointerEventData.pointerPress = null;
            }

            void TryTriggerPointerEnter(GameObject obj, PointerEventData pointerEventData)
            {
                if (_currentHovering.Add(obj))
                {
                    ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerEnterHandler);
                }
            }

            void TryTriggerPointerExit(GameObject obj, PointerEventData pointerEventData)
            {
                if (_currentHovering.Remove(obj))
                {
                    ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerExitHandler);
                }
            }

            void TryTriggerPointerDown(GameObject obj, PointerEventData pointerEventData)
            {
                if (_currentPressing != null && _currentPressing.Add(obj))
                {
                    ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerDownHandler);
                }
            }

            void TryTriggerPointerUp(GameObject obj, PointerEventData pointerEventData)
            {
                if (_currentPressing != null && _currentPressing.Remove(obj))
                {
                    ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerUpHandler);
                }
            }

            void TryTriggerPointerClick(GameObject obj, PointerEventData pointerEventData)
            {
                if (currentClicking != null && currentClicking.Add(obj))
                {
                    ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerClickHandler);
                }
            }
        }

        private GameObject GetPointerPressableObject(List<RaycastResult> results)
        {
            if (results == null || results.Count < 1)
                return null;

            foreach (var result in results)
            {
                var obj = result.gameObject;
                if (obj.GetComponent<IPointerDownHandler>() != null)
                {
                    return obj;
                }
            }

            return null;
        }

        private GameObject GetPointerClickableObject(List<RaycastResult> results)
        {
            if (results == null || results.Count < 1)
                return null;

            foreach (var result in results)
            {
                var obj = result.gameObject;
                if (obj.GetComponent<IPointerClickHandler>() != null)
                {
                    return obj;
                }
            }

            return null;
        }
        #endregion

        #region Private
        private bool IsPinching()
        {
            if (_currentHand == null)
                return false;

            var pinchStrength = _currentHand.PinchStrength;
            var isPinching = pinchStrength > GameGeneralConfig.GetData().PinchStrengthThreshold;
            return isPinching;
        }

        private bool HasValidHandRelativeWorldPosition(Hand hand, out Vector3 position)
        {
            position = Vector3.zero;

            if (hand == null)
                return false;

            position = hand.WristPosition - MotionTracker.transform.position;

            if (position == Vector3.zero)
                return false;

            if (position.sqrMagnitude > 1000)
                return false;

            return true;
        }
        #endregion

        #region Public
        public void ShowCursor()
        {
            UICursor.Enable();
        }

        public void HideCursor()
        {
            UICursor.Disable();
        }
        #endregion

        #region Public Static
        public static bool HasAnyKeyDown()
        {
            return Input.anyKeyDown;
        }

        public static bool HasAnyKey()
        {
            return Input.anyKey;
        }

        public static bool GetMouseButtonDown(int button)
        {
            return Input.GetMouseButtonDown(button);
        }

        public static bool GetMouseButtonUp(int button)
        {
            return Input.GetMouseButtonUp(button);
        }

        public static bool GetMouseButton(int button)
        {
            return Input.GetMouseButton(button);
        }

        public static bool GetKeyDown(KeyCode keyCode)
        {
            return Input.GetKeyDown(keyCode);
        }

        public static bool GetKeyUp(KeyCode keyCode)
        {
            return Input.GetKeyUp(keyCode);
        }

        public static bool GetKey(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }
        #endregion
    }
}