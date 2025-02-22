using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class UICursor
    /// @brief Controls the in-game UI cursor, handling its position, click events, and interactive states.
    /// 
    /// The `UICursor` class is responsible for managing the visual behavior of the in-game cursor. It tracks cursor movement,
    /// handles click-and-hold actions, and provides visual feedback, such as a loading circle when the cursor is held on a clickable target.

    public class UICursor : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether the cursor is enabled and visible in the UI.
        /// </summary>
        public bool IsEnabled { get; private set; } = true;

        /// <summary>
        /// Event triggered when the cursor is clicked.
        /// </summary>
        public static Action OnClick;

        [SerializeField] private Image _cursor;
        [SerializeField] private Image _circle;

        private RectTransform _rt;
        private float _nextCanHoldTime;
        private bool _isOnClickable;
        private float _idleDuration;
        private bool _isHolding;
        private float _holdingTime;
        private Vector3 _holdingStartPosition;
        private Vector3 _lastTickPosition;

        /// <summary>
        /// Initializes the cursor by setting up the RectTransform and variables.
        /// </summary>
        private void Awake()
        {
            _rt = (RectTransform)transform;
        }

        /// <summary>
        /// Updates the cursor every frame, checking if it is idle, being held, or interacting with clickable elements.
        /// </summary>
        private void Update()
        {
            var tickPosition = GetPosition();
            var deltaTickPosition = tickPosition - _lastTickPosition;
            _lastTickPosition = tickPosition;
            var isIdle = deltaTickPosition.sqrMagnitude < InputConfig.GetData().CursorHoldingMaxScreenOffset;
            if (isIdle && _isOnClickable && InputManager.Instance.CanClick())
            {
                if (_idleDuration < -.5f)
                {
                    _idleDuration = 0;
                }

                _idleDuration += Time.deltaTime;
            }
            else
            {
                _idleDuration = -1;
            }

            var isHolding = _idleDuration >= InputConfig.GetData().CursorPreHoldingCooldownTime && Time.time >= _nextCanHoldTime;
            var isInterrupted = _isHolding && (!IsIdle() || Vector3.Magnitude(_holdingStartPosition - GetPosition()) > InputConfig.GetData().CursorHoldingMaxScreenOffset);
            if (isInterrupted)
            {
                isHolding = false;
                _nextCanHoldTime = Time.time + InputConfig.GetData().CursorPreHoldingCooldownTime; // holding interrupted, need cooldown
            }

            if (!_isHolding && isHolding)
            {
                StartHolding();
            }
            else if (_isHolding && !isHolding)
            {
                StopHolding();
            }

            _isHolding = isHolding;

            if (_isHolding)
            {
                _cursor.enabled = false;
                _circle.enabled = true;

                _holdingTime += Time.deltaTime;

                var cursorClickHoldingTimeMax = InputConfig.GetData().CursorClickHoldingTimeMax;
                if (_holdingTime < cursorClickHoldingTimeMax)
                {
                    var heldCircleFill = (_holdingTime % cursorClickHoldingTimeMax) / cursorClickHoldingTimeMax;
                    _circle.fillAmount = Mathf.Clamp01(heldCircleFill);
                }
                else
                {
                    _holdingTime = 0;
                    _idleDuration = -1;
                    _nextCanHoldTime = Time.time + InputConfig.GetData().CursorPreHoldingCooldownTime;

                    OnClick?.Invoke();
                }
            }
            else
            {
                _cursor.enabled = true;
                _circle.enabled = false;
            }
        }

        /// <summary>
        /// Starts the holding action, resetting the circle's fill and position.
        /// </summary>
        private void StartHolding()
        {
            _isHolding = true;
            _circle.fillAmount = 0;
            _holdingStartPosition = GetPosition();
        }

        /// <summary>
        /// Stops the holding action, resetting the holding time and circle fill.
        /// </summary>
        private void StopHolding()
        {
            _isHolding = false;
            _circle.fillAmount = 0;
            _holdingTime = 0;
        }

        /// <summary>
        /// Checks if the cursor is idle.
        /// </summary>
        /// <returns>True if the cursor is idle, otherwise false.</returns>
        private bool IsIdle()
        {
            if (_idleDuration < -.5f)
                return false;

            return true;
        }

        /// <summary>
        /// Sets whether the cursor is currently over a clickable UI element.
        /// </summary>
        /// <param name="isOnClickable">True if the cursor is over a clickable element, otherwise false.</param>
        public void SetOnClickable(bool isOnClickable)
        {
            _isOnClickable = isOnClickable;
        }

        /// <summary>
        /// Returns the current position of the cursor.
        /// For the cursor on Canvas - Overlay, all positions are equal.
        /// </summary>
        /// <returns>The position of the cursor in screen space.</returns>
        public Vector3 GetPosition()
        {
            return _rt.position;
        }

        /// <summary>
        /// Sets the position of the cursor in screen space.
        /// </summary>
        /// <param name="position">The new position to set for the cursor.</param>
        public void SetPosition(Vector2 position)
        {
            _rt.position = new Vector2(position.x, position.y);
        }

        /// <summary>
        /// Enables the cursor, making it visible and active.
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
            _rt.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables the cursor, making it invisible and inactive.
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
            _rt.gameObject.SetActive(false);
        }
    }
}