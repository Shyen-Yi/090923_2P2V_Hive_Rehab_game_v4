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
                Logger.LogError($"Update CurrentLevel -> {CurrentLevel}");
            }
        }
        private int _currentLevel;

        public int LatestLevelPlayed { get; private set; }

        private Dictionary<int, Dictionary<int, int>> _levelStreaks;

        #region Lifecycle
        public void OnInit()
        {
            _levelStreaks = new Dictionary<int, Dictionary<int, int>>();

            InitLevel();

            var latestLevelPlayedKey = GetLatestLevelPlayedKey();
            if (PlayerPrefs.HasKey(latestLevelPlayedKey))
            {
                LatestLevelPlayed = PlayerPrefs.GetInt(latestLevelPlayedKey);
            }
            else
            {
                LatestLevelPlayed = 0;
            }

            Logger.LogError($"LevelManager.OnInit - LatestLevelPlayed: {LatestLevelPlayed} | _levelStreaks: {string.Join(",", _levelStreaks)}");

            SettingManager.OnDisplayNameUpdated += OnDisplayNameUpdated;
        }

        public void OnDispose()
        {
            _levelStreaks.Clear();
            _levelStreaks = null;

            SettingManager.OnDisplayNameUpdated -= OnDisplayNameUpdated;
        }
        #endregion

        private string GetLevelStreaksKey()
        {
            var key = SettingManager.Instance.IsDefaultUser
                ? ""
                : $"{SettingManager.Instance.DisplayName}_{PlayerPrefKeys.LevelStreaks}";

            return key;
        }

        private string GetLatestLevelPlayedKey()
        {
            var key = SettingManager.Instance.IsDefaultUser
                ? ""
                : $"{SettingManager.Instance.DisplayName}_{PlayerPrefKeys.LatestLevelPlayed}";

            return key;
        }

        private void InitLevel()
        {
            _levelStreaks.Clear();

            CurrentLevel = CoreGameLevelConfig.MinLevel;

            var key = GetLevelStreaksKey();

            if (PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                var playedDaysStorage = JsonConvert.DeserializeObject<LevelStreaks>(json);
                if (playedDaysStorage != null)
                {
                    var dict = playedDaysStorage.dict;
                    var levels = new List<int>(dict.Keys);
                    levels.Sort();

                    var maxLevel = CoreGameLevelConfig.MinLevel;
                    var maxLevelStreak = 0;

                    foreach (var day in dict.Keys)
                    {
                        _levelStreaks[day] = new Dictionary<int, int>();

                        var levelDict = dict[day];
                        foreach (var level in levelDict.Keys)
                        {
                            var streak = levelDict[level];
                            _levelStreaks[day][level] = streak;

                            if (streak > 0 && level > maxLevel)
                            {
                                maxLevel = level;
                                maxLevelStreak = streak;
                            }
                        }
                    }

                    var requiredStreakToPass = CoreGameLevelConfig.GetLevelData(maxLevel).RequiredDailySuccessToPass;
                    var isPassed = maxLevelStreak >= requiredStreakToPass;

                    if (!isPassed)
                    {
                        CurrentLevel = maxLevel;
                    }
                    else
                    {
                        CurrentLevel = Mathf.Max(maxLevel + 1, CoreGameLevelConfig.MaxLevel);
                    }
                }
            }

            Logger.LogError($"InitLevel - CurrentLevel: {CurrentLevel}");
        }

        public void OnLevelStarted(int level)
        {
            if (LatestLevelPlayed != level)
            {
                var currentGameDay = TimeManager.Instance.GetCurrentGameDay().day;
                if (!_levelStreaks.TryGetValue(currentGameDay, out var levelDict))
                {
                    _levelStreaks[currentGameDay] = new Dictionary<int, int>();
                }

                levelDict[level] = 0;
            }

            LatestLevelPlayed = level;

            if (!SettingManager.Instance.IsDefaultUser)
            {
                PlayerPrefs.SetInt(GetLatestLevelPlayedKey(), LatestLevelPlayed);
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
            PlayerPrefs.SetInt(GetLatestLevelPlayedKey(), LatestLevelPlayed);

            var currentGameDay = TimeManager.Instance.GetCurrentGameDay().day;
            if (!_levelStreaks.TryGetValue(currentGameDay, out var levelDict))
            {
                _levelStreaks[currentGameDay] = new Dictionary<int, int>();
            }

            ++_levelStreaks[LatestLevelPlayed];

            var levelConfigData = CoreGameLevelConfig.GetLevelData(level);
            if (_levelStreaks[LatestLevelPlayed] >= levelConfigData.RequiredDailySuccessToPass)
            {
                CurrentLevel = Mathf.Max(level + 1, CoreGameLevelConfig.MaxLevel);
            }

            var json = JsonConvert.SerializeObject(new LevelStreaks() { dict = _levelStreaks });
            PlayerPrefs.SetString(GetLevelStreaksKey(), json);

            Logger.LogError($"Save Latest Level Passed Streak - _levelStreaks: {string.Join(",", _levelStreaks)} | json: {json}");
        }

        private void OnDisplayNameUpdated()
        {
            InitLevel();
        }

        private void UpdateLevelStreak(int level, int streak)
        {
            var currentGameDay = TimeManager.Instance.GetCurrentGameDay().day;
            if (!_levelStreaks.TryGetValue(currentGameDay, out var levelDict))
            {
                levelDict = new Dictionary<int, int>();
                _levelStreaks[currentGameDay] = levelDict;
            }

            levelDict[level] = streak;
        }
    }
}