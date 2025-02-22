using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    public struct LevelStats
    {
        /// <summary>
        /// The level the player is on.
        /// </summary>
        public int level;

        /// <summary>
        /// The number of asteroids collected in the level.
        /// </summary>
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
        /// <summary>
        /// The set of game days the player has played.
        /// </summary>
        public HashSet<GameDay> days;
    }

    /// @ingroup Core
    /// @class StatsManager
    /// @brief Manages player statistics, including tracking played days, performance streaks, and level statistics.
    /// 
    /// The `StatsManager` class tracks various statistics related to the player's progress in the game. It tracks days the player has played,
    /// calculates the player's playing streak, and manages performance ratings after completing levels. It also saves and loads this data to
    /// and from `PlayerPrefs`, ensuring persistent player statistics across sessions.
    public class StatsManager : SingletonBase<StatsManager>, ICoreManager
    {
        #region Properties
        /// <summary>
        /// The player's current playing streak.
        /// </summary>
        public int PlayingStreak { get; private set; }

        /// <summary>
        /// The current performance type based on level completion.
        /// </summary>
        public PerformanceType PerformanceType { get; private set; }

        /// <summary>
        /// A description of the player's performance.
        /// </summary>
        public string PerformanceDesc { get; private set; }
        #endregion

        #region Fields
        private HashSet<GameDay> _playedDays; // Days the player has played
        private int _lastCollectedCount; // The last number of items or goals collected by the player
        #endregion

        #region Lifecycle
        /// <summary>
        /// Initializes the StatsManager by loading the player's played days, performance streak, and performance data.
        /// </summary>
        public void OnInit()
        {
            InitPlayedDays();
            InitPlayingStreak();
            InitPerformance();

            // Subscribe to display name updates
            SettingManager.OnDisplayNameUpdated += OnDisplayNameUpdated;
        }

        /// <summary>
        /// Disposes of the StatsManager by clearing the played days and unsubscribing from events.
        /// </summary>
        public void OnDispose()
        {
            _playedDays.Clear();
            _playedDays = null;

            SettingManager.OnDisplayNameUpdated -= OnDisplayNameUpdated;
        }
        #endregion

        #region Initialization Methods
        /// <summary>
        /// Reinitializes statistics when the display name is updated.
        /// </summary>
        private void OnDisplayNameUpdated()
        {
            InitPlayedDays();
            InitPlayingStreak();
            InitPerformance();
        }

        /// <summary>
        /// Initializes the played days, loading data from PlayerPrefs if available.
        /// </summary>
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

        /// <summary>
        /// Initializes the playing streak by calculating the number of consecutive days the player has played.
        /// </summary>
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

        /// <summary>
        /// Initializes the performance statistics, including loading the last performance type and description from PlayerPrefs.
        /// </summary>
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
        #endregion

        #region Public Methods
        /// <summary>
        /// Checks if the player played on a specific date.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>True if the player played on that date, otherwise false.</returns>
        public bool DidPlayOnDate(DateTime date)
        {
            var gameDay = new GameDay(date);
            return _playedDays.Contains(gameDay);
        }

        /// <summary>
        /// Handles the completion of a level and updates the player's performance based on the collected count.
        /// </summary>
        /// <param name="stats">The level statistics, including level number and collected count.</param>
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
        #endregion
    }
}