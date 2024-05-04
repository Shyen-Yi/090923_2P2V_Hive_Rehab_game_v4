using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.hive.projectr
{
    public class UICursor : MonoBehaviour
    {
        public bool IsEnabled { get; private set; } = true;

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

        private void Awake()
        {
            _rt = (RectTransform)transform;
        }

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

        private void StartHolding()
        {
            _isHolding = true;
            _circle.fillAmount = 0;
            _holdingStartPosition = GetPosition();
        }

        private void StopHolding()
        {
            _isHolding = false;
            _circle.fillAmount = 0;
            _holdingTime = 0;
        }

        private bool IsIdle()
        {
            if (_idleDuration < -.5f)
                return false;

            return true;
        }

        public void SetOnClickable(bool isOnClickable)
        {
            _isOnClickable = isOnClickable;
        }

        /// <summary>
        /// For the cursor on Canvas - Overlay, all positions are equal.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            return _rt.position;
        }

        public void SetPosition(Vector2 position)
        {
            _rt.position = new Vector2(position.x, position.y);
        }

        public void Enable()
        {
            IsEnabled = true;
            _rt.gameObject.SetActive(true);
        }

        public void Disable()
        {
            IsEnabled = false;
            _rt.gameObject.SetActive(false);
        }
    }
}