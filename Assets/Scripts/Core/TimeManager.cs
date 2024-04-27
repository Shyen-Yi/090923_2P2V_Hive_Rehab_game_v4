using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public class TimeManager : SingletonBase<TimeManager>, ICoreManager
    {
        #region Lifecycle
        public void OnInit()
        {

        }

        public void OnDispose()
        {

        }
        #endregion

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