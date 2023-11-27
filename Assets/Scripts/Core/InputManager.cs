using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class InputManager : SingletonBase<InputManager>, ICoreManager
    {
        public Vector3 CurrentScreenPos => _screenPos;

        private Vector3 _screenPos;
        private Vector3 _lastMousePos;

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Recenter();
            }

            var mousePos = Input.mousePosition;
            var deltaScreenPos = mousePos - _lastMousePos;
            _lastMousePos = mousePos;

            _screenPos += deltaScreenPos;
        }

        public void Recenter()
        {
            _screenPos = new Vector3(Screen.width / 2, Screen.height / 2);
        }

        #region Static
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

        public static bool GetMouseButton(int button)
        {
            return Input.GetMouseButton(button);
        }
        #endregion
    }
}