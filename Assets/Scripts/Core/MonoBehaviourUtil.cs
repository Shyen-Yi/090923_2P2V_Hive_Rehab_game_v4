using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class MonoBehaviourUtil
    /// @brief Provides utility methods for managing game updates, application focus, and pause events, with actions triggered for each frame type (Update, FixedUpdate, LateUpdate) and application state changes.
    /// 
    /// The `MonoBehaviourUtil` class is designed to provide centralized handling for common game loop events such as `Update`, `FixedUpdate`, and `LateUpdate`. 
    /// It also manages application state changes, like focus and pause events, allowing other systems to respond to these events via the use of `Action` delegates.
    public class MonoBehaviourUtil : MonoBehaviour
    {
        /// <summary>
        /// Gets the singleton instance of the MonoBehaviourUtil.
        /// </summary>
        public static MonoBehaviourUtil Instance { get; private set; }

        /// <summary>
        /// Indicates whether the application is currently focused.
        /// </summary>
        public bool IsFocused { get; private set; } = true;

        // Events for each update type
        public static Action OnUpdate;
        public static Action OnLateUpdate;
        public static Action OnUpdatePerSec;
        public static Action OnFixedUpdate;
        public static Action OnApplicationQuitEvent;
        public static Action OnApplicationFocusLost;
        public static Action OnApplicationFocusBack;

        private Coroutine _tickPerSecRoutine;

        /// <summary>
        /// Called when the MonoBehaviour is enabled.
        /// Initializes the singleton instance and starts the per-second update routine.
        /// </summary>
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

        /// <summary>
        /// Called when the MonoBehaviour is disabled.
        /// Stops the per-second update routine.
        /// </summary>
        private void OnDisable()
        {
            if (_tickPerSecRoutine != null)
            {
                StopCoroutine(_tickPerSecRoutine);
                _tickPerSecRoutine = null;
            }
        }

        /// <summary>
        /// Coroutine that triggers the `OnUpdatePerSec` event every second.
        /// </summary>
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

        /// <summary>
        /// Called every frame. Triggers the `OnUpdate` event.
        /// </summary>
        private void Update()
        {
            OnUpdate?.Invoke();
        }

        /// <summary>
        /// Called after all `Update` calls. Triggers the `OnLateUpdate` event.
        /// </summary>
        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        /// <summary>
        /// Called every fixed frame-rate frame. Triggers the `OnFixedUpdate` event.
        /// </summary>
        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        /// <summary>
        /// Called when the application is quitting. Triggers the `OnApplicationQuitEvent` event.
        /// </summary>
        private void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Invoke();
        }

        /// <summary>
        /// Called when the application is paused or resumed. Triggers events based on focus and pause state.
        /// </summary>
        /// <param name="pause">Indicates whether the application is paused.</param>
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

        /// <summary>
        /// Called when the application gains or loses focus. Triggers events based on the focus state.
        /// </summary>
        /// <param name="focus">Indicates whether the application has gained focus.</param>
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