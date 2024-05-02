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
        private Hand _currentHand;

        private bool _isPinching;

        #region Lifecycle
        public void OnInit()
        {
            _eventSystem = EventSystem.current;
            _pointerEventData = new PointerEventData(_eventSystem);

            MonoBehaviourUtil.OnUpdate += Tick;
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
                    ExecuteEvents.Execute(_currentTarget, _pointerEventData, ExecuteEvents.pointerExitHandler);
                    //Logger.LogError($"### Pointer Exit - _currentTarget: {_currentTarget.name}");
                }
                if (newTarget != null)
                {
                    ExecuteEvents.Execute(newTarget, _pointerEventData, ExecuteEvents.pointerEnterHandler);
                    //Logger.LogError($"### Pointer Enter - _currentTarget: {newTarget.name}");
                }
                _currentTarget = newTarget;
            }

            var thumb = _currentHand.Fingers[0];
            var index = _currentHand.Fingers[1];
            var isPinching = IsPinching();
            if (isPinching)
            {
                if (!_isPinching) // press down
                {
                    if (raycastResults.Count > 0)
                    {
                        _pointerEventData.pressPosition = _pointerEventData.position;
                        _pointerEventData.pointerPressRaycast = raycastResults[0];
                        if (_currentTarget != null)
                        {
                            var pressObject = ExecuteEvents.ExecuteHierarchy(_currentTarget, _pointerEventData, ExecuteEvents.pointerDownHandler);
                            _pointerEventData.pointerPress = pressObject;

                            //Logger.LogError($"### Pointer Down - _currentTarget: {_currentTarget.name} | pressObject: {pressObject?.name ?? "NONE"}");
                        }
                    }
                }
            }
            else
            {
                if (_isPinching) // release
                {
                    if (_pointerEventData.pointerPress != null)
                    {
                        ExecuteEvents.Execute(_pointerEventData.pointerPress, _pointerEventData, ExecuteEvents.pointerUpHandler);
                        //Logger.LogError($"### Pointer Up - {_pointerEventData.pointerPress.name}");

                        var clickObject = ExecuteEvents.ExecuteHierarchy(_currentTarget, _pointerEventData, ExecuteEvents.pointerClickHandler);
                        if (clickObject != null && clickObject == _pointerEventData.pointerPress)
                        {
                            //Logger.LogError($"### Pointer Click - {_pointerEventData.pointerPress.name}");
                        }
                    }

                    _pointerEventData.pointerPress = null;
                }
            }

            _isPinching = isPinching;
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