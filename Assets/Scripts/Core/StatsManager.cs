using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        }

        public void OnDispose()
        {
            _playedDays.Clear();
            _playedDays = null;
        }
        #endregion

        private string GetPlayedDaysKey()
        {
            var key = SettingManager.Instance.IsDefaultUser
                ? ""
                : $"{SettingManager.Instance.DisplayName}_{PlayerPrefKeys.PlayedDays}";

            return key;
        }

        private string GetLastPerformanceKey()
        {
            var key = SettingManager.Instance.IsDefaultUser
                ? ""
                : $"{SettingManager.Instance.DisplayName}_{PlayerPrefKeys.LastPerformance}";

            return key;
        }

        private string GetLastPerformanceDescKey()
        {
            var key = SettingManager.Instance.IsDefaultUser
                ? ""
                : $"{SettingManager.Instance.DisplayName}_{PlayerPrefKeys.LastPerformanceDesc}";

            return key;
        }

        private void InitPlayedDays()
        {
            if (_playedDays == null)
            {
                _playedDays = new HashSet<GameDay>();
            }

            if (PlayerPrefs.HasKey(GetPlayedDaysKey()))
            {
                var json = PlayerPrefs.GetString(PlayerPrefKeys.PlayedDays);
                var playedDaysStorage = JsonUtility.FromJson<PlayedDays>(json);
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

                var json = JsonUtility.ToJson(_playedDays);
                PlayerPrefs.SetString(PlayerPrefKeys.PlayedDays, json);
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
            var lastPerformanceKey = GetLastPerformanceKey();
            var lastPerformanceDescKey = GetLastPerformanceDescKey();

            if (PlayerPrefs.HasKey(lastPerformanceKey) &&
                PlayerPrefs.HasKey(lastPerformanceDescKey))
            {
                PerformanceType = (PerformanceType)PlayerPrefs.GetInt(lastPerformanceKey);
                PerformanceDesc = PlayerPrefs.GetString(lastPerformanceDescKey);
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

                PlayerPrefs.SetInt(GetLastPerformanceKey(), (int)PerformanceType);
                PlayerPrefs.SetString(GetLastPerformanceDescKey(), PerformanceDesc);
            }
            else
            {
                Logger.LogError($"No performance data defined for performance type: {performance}");
            }

            _lastCollectedCount = stats.collectedCount;
        }
    }
}