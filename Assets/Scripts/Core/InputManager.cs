using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class InputManager : SingletonBase<InputManager>, ICoreManager
    {
        public Vector3 CursorScreenPos => Input.mousePosition;
        public float IdleDuration => _idleDuration;

        private Vector3 _lastCursorScreenPos;
        private float _idleDuration;

        private static readonly float IdleDistanceThreshold = .01f;

        #region Lifecycle
        public void OnInit()
        {
            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }

        private void Tick()
        {
            var deltaCursorScreenPos = CursorScreenPos - _lastCursorScreenPos;
            _lastCursorScreenPos = CursorScreenPos;

            if (deltaCursorScreenPos.sqrMagnitude < IdleDistanceThreshold)
            {
                _idleDuration += Time.deltaTime;
            }
            else
            {
                _idleDuration = 0;
            }
        }
        #endregion

        #region Public Instance
        public void ShowCursor()
        {
            Cursor.visible = true;
        }

        public void HideCursor()
        {
            Cursor.visible = false;
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
        #endregion
    }
}