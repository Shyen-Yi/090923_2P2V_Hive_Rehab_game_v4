using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public class MonoBehaviourUtil : MonoBehaviour
    {
        public static MonoBehaviourUtil Instance { get; private set; }
        
        public bool IsFocused { get; private set; } = true;

        public static Action OnUpdate;
        public static Action OnLateUpdate;
        public static Action OnUpdatePerSec;
        public static Action OnFixedUpdate;
        public static Action OnApplicationQuitEvent;
        public static Action OnApplicationFocusLost;
        public static Action OnApplicationFocusBack;

        private Coroutine _tickPerSecRoutine;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (_tickPerSecRoutine != null)
            {
                StopCoroutine(_tickPerSecRoutine);
            }
            _tickPerSecRoutine = StartCoroutine(TickPerSecRoutine());
        }

        private void OnDisable()
        {
            if (_tickPerSecRoutine != null)
            {
                StopCoroutine(_tickPerSecRoutine);
                _tickPerSecRoutine = null;
            }
        }

        private IEnumerator TickPerSecRoutine()
        {
            long triggeredSec = -1;
            long secToTrigger = -1;
            while (Application.isPlaying)
            {
                if (Time.time % 1 < .001f)
                {
                    secToTrigger = (int)Time.time;
                }
                if (secToTrigger > triggeredSec)
                {
                    triggeredSec = secToTrigger;
                    OnUpdatePerSec?.Invoke();
                }

                yield return null;
            }
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Invoke();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (IsFocused)
                {
                    OnApplicationFocusLost?.Invoke();
                    IsFocused = false;

                    Logger.Log($"Focus Lost");
                }
            }
            else
            {
                if (!IsFocused)
                {
                    OnApplicationFocusBack?.Invoke();
                    IsFocused = true;
                }
            }

            IsFocused = !pause;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (IsFocused)
                {
                    OnApplicationFocusLost?.Invoke();

                    Logger.Log($"Focus Lost");
                }
            }
            else
            {
                if (!IsFocused)
                {
                    OnApplicationFocusBack?.Invoke();

                    Logger.Log($"Focus Back");
                }
            }

            IsFocused = focus;
        }
    }
}