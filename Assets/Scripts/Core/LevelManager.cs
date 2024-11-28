using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    [Serializable]
    public class LevelStreaks
    {
        public Dictionary<int, Dictionary<int, int>> dict; // key1: gameday.day, key2: level, value: streak
    }

    public class LevelManager : SingletonBase<LevelManager>, ICoreManager
    {
        public int CurrentLevel
        {
            get => _currentLevel;
            private set
            {
                _currentLevel = value;
            }
        }
        private int _currentLevel;

        public int LatestLevelPlayed { get; private set; }

        private Dictionary<int, Dictionary<int, int>> _dailyLevelStreaks; // key1: gameday.day, key2: level, value: streak

        #region Lifecycle
        public void OnInit()
        {
            _dailyLevelStreaks = new Dictionary<int, Dictionary<int, int>>();

            InitLevel();

            var latestLevelPlayedKey = PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LatestLevelPlayed);
            if (PlayerPrefsUtil.HasKey(latestLevelPlayedKey))
            {
                LatestLevelPlayed = PlayerPrefsUtil.GetInt(latestLevelPlayedKey);
            }
            else
            {
                LatestLevelPlayed = 0;
            }

            SettingManager.OnDisplayNameUpdated += OnDisplayNameUpdated;
        }

        public void OnDispose()
        {
            _dailyLevelStreaks.Clear();
            _dailyLevelStreaks = null;

            SettingManager.OnDisplayNameUpdated -= OnDisplayNameUpdated;
        }
        #endregion

        private void InitLevel()
        {
            _dailyLevelStreaks.Clear();

            CurrentLevel = CoreGameLevelConfig.MinLevel;

            try
            {
                var key = PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LevelStreaks);

                if (PlayerPrefsUtil.HasKey(key))
                {
                    var json = PlayerPrefsUtil.GetString(key);
                    var playedDaysStorage = JsonConvert.DeserializeObject<LevelStreaks>(json);
                    if (playedDaysStorage != null)
                    {
                        var dict = playedDaysStorage.dict;
                        var levels = new List<int>(dict.Keys);
                        levels.Sort();

                        var maxLevel = CoreGameLevelConfig.MinLevel;
                        var maxLevelStreak = 0;

                        var currentGameDay = TimeManager.Instance.GetCurrentGameDay();

                        foreach (var day in dict.Keys)
                        {
                            if (currentGameDay.day == day)
                            {
                                _dailyLevelStreaks[day] = new Dictionary<int, int>();
                            }

                            var levelDict = dict[day];
                            foreach (var level in levelDict.Keys)
                            {
                                var streak = levelDict[level];

                                if (currentGameDay.day == day)
                                {
                                    _dailyLevelStreaks[day][level] = streak;
                                }

                                if (streak > 0 && level > maxLevel)
                                {
                                    maxLevel = level;
                                    maxLevelStreak = streak;
                                }
                            }
                        }

                        var requiredStreakToPass = CoreGameLevelConfig.GetLevelData(maxLevel).RequiredDailyWinningStreakToPass;
                        var isLevelPassed = maxLevelStreak >= requiredStreakToPass;

                        if (!isLevelPassed)
                        {
                            CurrentLevel = maxLevel;
                        }
                        else
                        {
                            CurrentLevel = Mathf.Min(maxLevel + 1, CoreGameLevelConfig.MaxLevel);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            Logger.Log($"LevelManager::InitLevel - CurrentLevel: {CurrentLevel}");
        }

        public int GetLevelWinningStreak(int level)
        {
            if (_dailyLevelStreaks.TryGetValue(TimeManager.Instance.GetCurrentGameDay().day, out var levelDict) &&
                levelDict.TryGetValue(level, out var streak))
            {
                return streak;
            }

            return 0;
        }

        public void OnLevelStarted(int level)
        {
            if (LatestLevelPlayed != level)
            {
                var currentGameDay = TimeManager.Instance.GetCurrentGameDay().day;
                if (!_dailyLevelStreaks.TryGetValue(currentGameDay, out var levelDict))
                {
                    levelDict = new Dictionary<int, int>();
                    _dailyLevelStreaks[currentGameDay] = levelDict;
                }

                levelDict[level] = 0;
            }

            LatestLevelPlayed = level;

            if (!SettingManager.Instance.IsDefaultUser)
            {
                PlayerPrefsUtil.TrySetInt(PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LatestLevelPlayed), LatestLevelPlayed);
            }
        }

        public void OnLevelCompleted(int level, bool isPassed)
        {
            if (level != LatestLevelPlayed)
            {
                Logger.LogError($"Level completed {level} doesn't match the level started {LatestLevelPlayed}!");
                return;
            }

            // save latest level result
            LatestLevelPlayed = level;
            PlayerPrefsUtil.TrySetInt(PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LatestLevelPlayed), LatestLevelPlayed);

            var currentGameDay = TimeManager.Instance.GetCurrentGameDay().day;
            if (!_dailyLevelStreaks.TryGetValue(currentGameDay, out var levelDict))
            {
                levelDict = new Dictionary<int, int>();
                _dailyLevelStreaks[currentGameDay] = levelDict;
            }

            if (!levelDict.ContainsKey(level))
            {
                levelDict[level] = 0;
            }

            if (isPassed)
            {
                ++levelDict[level];

                var levelConfigData = CoreGameLevelConfig.GetLevelData(level);
                if (levelDict[level] >= levelConfigData.RequiredDailyWinningStreakToPass)
                {
                    CurrentLevel = Mathf.Min(level + 1, CoreGameLevelConfig.MaxLevel);
                }
            }
            else
            {
                levelDict.Remove(level);
            }

            var levelStreaks = new LevelStreaks() { dict = _dailyLevelStreaks };
            var json = JsonConvert.SerializeObject(levelStreaks);
            PlayerPrefs.SetString(PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LevelStreaks), json);
        }

        private void OnDisplayNameUpdated()
        {
            InitLevel();
        }
    }
}