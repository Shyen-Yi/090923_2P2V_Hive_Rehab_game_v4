using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public class TimeManager : SingletonBase<TimeManager>, ICoreManager
    {
        private GameDay _currentGameDay;

        public static Action OnGameDayUpdated;

        #region Lifecycle
        public void OnInit()
        {
            _currentGameDay = GetCurrentGameDay();

            MonoBehaviourUtil.OnUpdatePerSec += OnUpdatePerSec;
        }

        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdatePerSec -= OnUpdatePerSec;
        }
        #endregion

        private void OnUpdatePerSec()
        {
            var gameDay = GetCurrentGameDay();
            if (gameDay.day != _currentGameDay.day)
            {
                OnGameDayUpdated?.Invoke();
            }
            _currentGameDay = gameDay;
        }

        public GameDay GetCurrentGameDay()
        {
            return new GameDay(GetCurrentDateTime());
        }

        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}