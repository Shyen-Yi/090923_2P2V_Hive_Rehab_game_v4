using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Leap;

namespace com.hive.projectr
{
    /// <summary>
    /// @ingroup Core
    /// @class InputManager
    /// @brief Manages user input for the game, including cursor movement, clicking, and handling interactions with UI elements.
    /// 
    /// The `InputManager` class handles user input through various methods, such as cursor control, pointer events (clicks, hovers), 
    /// and tracking of the user's hand movement through a motion tracker (e.g., Leap Motion). It manages the behavior of the cursor 
    /// and updates the UI interaction, such as triggering pointer enter, exit, click, and other interactions on UI elements.
    /// </summary>
    public class InputManager : SingletonBase<InputManager>, ICoreManager
    {
        /// <summary>
        /// Gets the current screen position of the cursor.
        /// </summary>
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

        /// <summary>
        /// Gets the MotionTracker component, which tracks the user's hand movement.
        /// </summary>
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

        /// <summary>
        /// Gets the UICursor component, which represents the cursor's position on the screen.
        /// </summary>
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
        private Vector3 _prevCursorPos;
        private Vector3 _initCursorPos;
        private Vector3 _cursorPosOffset;
        private bool _hasMovedCursor;
        private bool _isCursorInitialized;
        private PointerEventData _pointerEventData;
        private EventSystem _eventSystem;
        private GameObject _currentTarget;
        private GameObject _currentClickable;
        private HashSet<GameObject> _currentPressing = new HashSet<GameObject>();
        private HashSet<GameObject> _currentHovering = new HashSet<GameObject>();
        private HashSet<GameObject> _currentClicking = new HashSet<GameObject>();
        private Hand _currentHand;
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();

        private bool _isPinching;

        #region Lifecycle
        /// <summary>
        /// Initializes the input manager, setting up the event system, cursor, and input handling.
        /// </summary>
        public void OnInit()
        {
            _eventSystem = EventSystem.current;
            _pointerEventData = new PointerEventData(_eventSystem);

            MonoBehaviourUtil.OnUpdate += Tick;
            UICursor.OnClick += OnClick;

            Cursor.visible = DebugConfig.GetData().ShowBuiltInCursor;
        }

        /// <summary>
        /// Disposes of the input manager by cleaning up event subscriptions and input handling.
        /// </summary>
        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
            UICursor.OnClick -= OnClick;
        }

        /// <summary>
        /// Handles the click event when the user clicks on a clickable object.
        /// </summary>
        private void OnClick()
        {
            var clickable = GetPointerClickableObject(_raycastResults);
            if (clickable != null)
            {
                TryTriggerPointerClick(clickable, _pointerEventData);
            }
        }

        /// <summary>
        /// Update method called each frame to handle cursor movement and input.
        /// </summary>
        private void Tick()
        {
            CursorTick();
        }

        /// <summary>
        /// Updates the cursor position, tracks the movement of the user's hand, and processes the cursor input.
        /// </summary>
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
                }

                if (DebugConfig.GetData().EnableKeyboardCursorControl)
                {
                    var horizontal = Input.GetAxisRaw("Horizontal");
                    var vertical = Input.GetAxisRaw("Vertical");
                    _cursorPos += new Vector3(horizontal, vertical, 0) * MotionTracker.Sensitivity * Time.deltaTime * 100;
                }

                if ((_prevCursorPos - _cursorPos).sqrMagnitude > 1)
                {
                    _hasMovedCursor = true;
                }

                _prevCursorPos = _cursorPos;

                UICursor.SetPosition(_cursorPos + _cursorPosOffset);

                CursorInputTick();
            }
        }

        /// <summary>
        /// Updates pointer events and triggers interactions such as pointer entering, exiting, clicking, and pressing on UI elements.
        /// </summary>
        private void CursorInputTick()
        {
            _pointerEventData.position = UICursor.GetPosition();

            // do raycasts and find all objects beneath the cursor
            _raycastResults.Clear();
            _eventSystem.RaycastAll(_pointerEventData, _raycastResults);

            var newTarget = _raycastResults.Count > 0 ? _raycastResults[0].gameObject : null;
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

            var newClickable = GetPointerClickableObject(_raycastResults);
            if (newClickable != null && _currentClickable == null)
            {
                TryTriggerPointerEnter(newTarget, _pointerEventData);
            }
            else if (newClickable == null && _currentClickable != null)
            {
                TryTriggerPointerExit(_currentClickable, _pointerEventData);
            }

            _currentClickable = newClickable;

            UICursor.SetOnClickable(_currentClickable != null);

            _currentClicking.Clear();

            if (_currentHand != null)
            {
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
                _pointerEventData.pointerPressRaycast = _raycastResults[0];

                var pressable = GetPointerPressableObject(_raycastResults);
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
        }

        /// <summary>
        /// Checks whether the cursor can trigger click events.
        /// </summary>
        /// <returns>True if the cursor meets all conditions and can trigger click events; otherwise, false.</returns>
        public bool CanClick()
        {
            return _hasMovedCursor;
        }

        /// <summary>
        /// Temporarily moves the cursor to the center of screen by adding a temporary offset (_cursorPosOffset).
        /// </summary>
        public void CenterCursor()
        {
            _cursorPosOffset = new Vector3(Screen.width / 2, Screen.height / 2, 0) - CursorScreenPosition;
            _hasMovedCursor = false; // need to move before clicking is enabled
        }

        /// <summary>
        /// Removes the temporary offset from the cursor (_cursorPosOffset).
        /// </summary>
        public void DecenterCursor()
        {
            _cursorPosOffset = Vector3.zero;
        }

        /// <summary>
        /// Gets the first GameObject underneath the cursor that can handle PointerDown events.
        /// Return null if no such object can be found.
        /// </summary>
        /// <param name="results">A list of RaycastResult objects from a raycast operation.</param>
        /// <returns>The first GameObject underneath the cursor that can handle PointerDown events.</returns>
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

        /// <summary>
        /// Gets the first GameObject underneath the cursor that can handle PointerClick events.
        /// Return null if no such object can be found.
        /// </summary>
        /// <param name="results">A list of RaycastResult objects from a raycast operation.</param>
        /// <returns>The first GameObject underneath the cursor that can handle PointerClick events.</returns>
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

        #region Pointer Event Trigger
        private void TryTriggerPointerEnter(GameObject obj, PointerEventData pointerEventData)
        {
            if (_currentHovering.Add(obj))
            {
                ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        private void TryTriggerPointerExit(GameObject obj, PointerEventData pointerEventData)
        {
            if (_currentHovering.Remove(obj))
            {
                ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        private void TryTriggerPointerDown(GameObject obj, PointerEventData pointerEventData)
        {
            if (_currentPressing != null && _currentPressing.Add(obj))
            {
                ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerDownHandler);
            }
        }

        private void TryTriggerPointerUp(GameObject obj, PointerEventData pointerEventData)
        {
            if (_currentPressing != null && _currentPressing.Remove(obj))
            {
                ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerUpHandler);
            }
        }

        private void TryTriggerPointerClick(GameObject obj, PointerEventData pointerEventData)
        {
            if (CanClick() && _currentClicking != null && _currentClicking.Add(obj))
            {
                ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerClickHandler);
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// Deprecated
        /// </summary>
        /// <returns></returns>
        private bool IsPinching()
        {
            return false;

            if (_currentHand == null)
                return false;

            var pinchStrength = _currentHand.PinchStrength;
            //var isPinching = pinchStrength > GameGeneralConfig.GetData().PinchStrengthThreshold;
            var isPinching = false;
            return isPinching;
        }

        /// <summary>
        /// Checks if a LeapMotion Hand object provides valid position data and can be used for valid game input.
        /// </summary>
        /// <param name="hand">LeapMotion Hand object.</param>
        /// <param name="position">Relative world position of hand to the motion tracking sensor.</param>
        /// <returns>True if the Hand object provides valid position data; otherwise, false.</returns>
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
        /// <summary>
        /// Shows the cursor.
        /// </summary>
        public void ShowCursor()
        {
            UICursor.Enable();
        }

        /// <summary>
        /// Hides the cursor.
        /// </summary>
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