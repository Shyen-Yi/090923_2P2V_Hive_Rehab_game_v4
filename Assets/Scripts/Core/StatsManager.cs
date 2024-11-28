using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    public struct LevelStats
    {
        public int level;
        public int collectedCount;

        public LevelStats(int level, int collectedCount)
        {
            this.level = level;
            this.collectedCount = collectedCount;
        }
    }

    [Serializable]
    public class PlayedDays
    {
        public HashSet<GameDay> days;
    }

    public class StatsManager : SingletonBase<StatsManager>, ICoreManager
    {
        #region Properties
        public int PlayingStreak { get; private set; }
        public PerformanceType PerformanceType { get; private set; }
        public string PerformanceDesc { get; private set; }
        #endregion

        #region Fields
        private HashSet<GameDay> _playedDays;
        private int _lastCollectedCount;
        #endregion

        #region Lifecycle
        public void OnInit()
        {
            InitPlayedDays();
            InitPlayingStreak();
            InitPerformance();

            SettingManager.OnDisplayNameUpdated += OnDisplayNameUpdated;
        }

        public void OnDispose()
        {
            _playedDays.Clear();
            _playedDays = null;

            SettingManager.OnDisplayNameUpdated -= OnDisplayNameUpdated;
        }
        #endregion

        private void OnDisplayNameUpdated()
        {
            InitPlayedDays();
            InitPlayingStreak();
            InitPerformance();
        }

        private void InitPlayedDays()
        {
            if (_playedDays == null)
            {
                _playedDays = new HashSet<GameDay>();
            }

            try
            {
                var key = PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.PlayedDays);

                if (PlayerPrefsUtil.HasKey(key))
                {
                    var json = PlayerPrefsUtil.GetString(key);
                    var playedDaysStorage = JsonConvert.DeserializeObject<PlayedDays>(json);
                    if (playedDaysStorage != null)
                    {
                        var playedDays = playedDaysStorage.days;
                        if (playedDays != null)
                        {
                            foreach (var playedDay in playedDays)
                            {
                                _playedDays.Add(playedDay);
                            }
                        }
                    }
                }

                var currentDay = TimeManager.Instance.GetCurrentGameDay();
                if (!_playedDays.Contains(currentDay))
                {
                    _playedDays.Add(currentDay);

                    var json = JsonConvert.SerializeObject(new PlayedDays() { days = _playedDays });
                    PlayerPrefsUtil.TrySetString(key, json);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        private void InitPlayingStreak()
        {
            var streak = 0;
            GameDay adjacentPlayedDay = new GameDay();

            var playedDays = new List<GameDay>(_playedDays);
            playedDays.Sort();

            for (var i = _playedDays.Count - 1; i >= 0; --i)
            {
                var gameDay = playedDays[i];

                if (streak == 0)
                {
                    ExtendStreak(gameDay);
                    continue;
                }

                if (gameDay.day == adjacentPlayedDay.day - 1)
                {
                    ExtendStreak(gameDay);
                }
                else
                {
                    var dateTime = gameDay.ToDateTime();
                    var adjacentDateTime = adjacentPlayedDay.ToDateTime();
                    var totalDaysInYear = DateTime.IsLeapYear(dateTime.Year) ? 366 : 365;

                    if (dateTime.Year == adjacentDateTime.Year - 1 &&
                        dateTime.DayOfYear == totalDaysInYear &&
                        adjacentDateTime.DayOfYear == 1)
                    {
                        ExtendStreak(gameDay);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            PlayingStreak = streak;

            void ExtendStreak(GameDay gameDay)
            {
                adjacentPlayedDay = gameDay;
                ++streak;
            }
        }

        private void InitPerformance()
        {
            var lastPerformanceKey = PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LastPerformance);
            var lastPerformanceDescKey = PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LastPerformanceDesc);

            if (PlayerPrefsUtil.HasKey(lastPerformanceKey) &&
                PlayerPrefsUtil.HasKey(lastPerformanceDescKey))
            {
                PerformanceType = (PerformanceType)PlayerPrefsUtil.GetInt(lastPerformanceKey);
                PerformanceDesc = PlayerPrefsUtil.GetString(lastPerformanceDescKey);
            }
            else
            {
                var data = PerformanceConfig.GetData(PerformanceType.None);
                if (data != null)
                {
                    PerformanceType = data.Type;
                    PerformanceDesc = data.Desc;
                }
                else
                {
                    Logger.LogError($"No defined performance data for type: {PerformanceType}");
                }
            }
        }

        public bool DidPlayOnDate(DateTime date)
        {
            var gameDay = new GameDay(date);
            return _playedDays.Contains(gameDay);
        }

        public void OnLevelCompleted(LevelStats stats)
        {
            var performance = PerformanceType.None;

            if (stats.collectedCount > _lastCollectedCount)
            {
                performance = PerformanceType.Excellent;
            }
            else
            {
                performance = PerformanceType.Good;
            }

            var data = PerformanceConfig.GetData(performance);
            if (data != null)
            {
                PerformanceType = data.Type;

                switch (PerformanceType)
                {
                    case PerformanceType.Excellent:
                        PerformanceDesc = string.Format(data.Desc, stats.collectedCount - _lastCollectedCount);
                        break;
                    default:
                        PerformanceDesc = data.Desc;
                        break;
                }

                PlayerPrefsUtil.TrySetInt(PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LastPerformance), (int)PerformanceType);
                PlayerPrefsUtil.TrySetString(PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LastPerformanceDesc), PerformanceDesc);
            }
            else
            {
                Logger.LogError($"No performance data defined for performance type: {performance}");
            }

            _lastCollectedCount = stats.collectedCount;
        }
    }
}