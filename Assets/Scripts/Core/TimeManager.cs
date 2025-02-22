using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class TimeManager
    /// @brief Manages the in-game time system, including tracking game days and triggering events when the day changes.
    /// 
    /// The `TimeManager` class is responsible for managing the in-game time system. It tracks the current game day and triggers events
    /// when the game day updates. The class provides functionality to get the current game day and current real-world date, and it 
    /// notifies other systems when the game day changes.
    public class TimeManager : SingletonBase<TimeManager>, ICoreManager
    {
        private GameDay _currentGameDay;

        /// <summary>
        /// Event triggered when the game day updates.
        /// </summary>
        public static Action OnGameDayUpdated;

        #region Lifecycle
        /// <summary>
        /// Initializes the TimeManager by setting the current game day and subscribing to the update event.
        /// </summary>
        public void OnInit()
        {
            _currentGameDay = GetCurrentGameDay();

            MonoBehaviourUtil.OnUpdatePerSec += OnUpdatePerSec;
        }

        /// <summary>
        /// Disposes of the TimeManager by unsubscribing from the update event.
        /// </summary>
        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdatePerSec -= OnUpdatePerSec;
        }
        #endregion

        /// <summary>
        /// Called every second, checks if the game day has changed and triggers the game day update event.
        /// </summary>
        private void OnUpdatePerSec()
        {
            var gameDay = GetCurrentGameDay();
            if (gameDay.day != _currentGameDay.day)
            {
                OnGameDayUpdated?.Invoke();
            }
            _currentGameDay = gameDay;
        }

        /// <summary>
        /// Gets the current game day.
        /// </summary>
        /// <returns>The current game day instance.</returns>
        public GameDay GetCurrentGameDay()
        {
            return new GameDay(GetCurrentDateTime());
        }

        /// <summary>
        /// Gets the current real-world date and time.
        /// </summary>
        /// <returns>The current real-world date and time.</returns>
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}