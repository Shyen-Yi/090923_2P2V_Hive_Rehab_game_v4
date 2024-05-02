using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
                    return UICursor.GetScreenPosition();
                }

                return Vector3.zero;
            }
        }

        private Vector3 _cursorPos;
        private Vector3 _initCursorPos;
        private bool _isCursorInitialized;

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

        private PointerEventData pointerEventData;
        private EventSystem eventSystem;

        #region Lifecycle
        public void OnInit()
        {
            eventSystem = EventSystem.current;

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }

        private void Tick()
        {
            if (UICursor != null && UICursor.IsEnabled && MotionTracker != null)
            {
                CursorMovementTick();
                CursorInputTick();
            }
        }

        private void CursorMovementTick()
        {
            if (TryGetHandRelativeWorldPos(out var worldPos))
            {
                var cursorPos = worldPos * MotionTracker.Sensitivity * MotionTracker.BaseSensitivityScale;

                if (!_isCursorInitialized)
                {
                    _isCursorInitialized = true;
                    _initCursorPos = cursorPos;
                }

                _cursorPos = cursorPos - _initCursorPos + MotionTracker.Offset * MotionTracker.BaseSensitivityScale;
            }

            UICursor.SetScaledAndCenteredPosition(_cursorPos);
        }

        private void CursorInputTick()
        {
            if (GetKeyDown(KeyCode.Space))
            {
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = UICursor.GetScreenPosition();
                ExecuteEvents.Execute(UICursor.gameObject, pointerEventData, ExecuteEvents.pointerDownHandler);

                Logger.LogError($"Press down - cursor world: {UICursor.GetWorldPosition()} | cursor screen: {UICursor.GetScreenPosition()}");
            }

            if (GetKeyUp(KeyCode.Space))
            {
                ExecuteEvents.Execute(UICursor.gameObject, pointerEventData, ExecuteEvents.pointerUpHandler);

                Logger.LogError($"Press up - cursor world: {UICursor.GetWorldPosition()} | cursor screen: {UICursor.GetScreenPosition()}");
            }
        }
        #endregion

        #region Private
        private bool TryGetHandRelativeWorldPos(out Vector3 position)
        {
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
                            position = rightHand.WristPosition - MotionTracker.transform.position;
                            return true;
                        }
                        var leftHand = handModelPair.Left.GetLeapHand();
                        if (leftHand != null)
                        {
                            position = leftHand.WristPosition - MotionTracker.transform.position;
                            return true;
                        }
                    }
                }
            }

            position = Vector3.zero;
            return false;
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