using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    [Serializable]
    public struct LevelStreakData
    {
        public int level;
        public int streak;

        public LevelStreakData(int level, int streak)
        {
            this.level = level;
            this.streak = streak;
        }
    }

    /// @ingroup Core
    /// @class LevelManager
    /// @brief Manages the player's progression through levels, including level streaks, the current level, and the player's progress.
    ///
    /// The `LevelManager` class is responsible for managing the player's current level, tracking level streaks (the number of
    /// times the player has completed a level successfully), and determining the next level based on the player's streaks and progress.
    /// It handles initialization of level data, updating the current level, and storing player progress in `PlayerPrefs`.
    public class LevelManager : SingletonBase<LevelManager>, ICoreManager
    {
        /// <summary>
        /// Gets the current level the player is at.
        /// </summary>
        public int CurrentLevel
        {
            get
            {
                return _currentLevelStreakData.level;
            }
        }

        /// <summary>
        /// Gets the latest level the player has completed.
        /// </summary>
        public int LatestLevelPlayed { get; private set; }

        /// <summary>
        /// Stores the latest level the player should play and the player's current winning streak.
        /// </summary>
        private LevelStreakData _currentLevelStreakData;

        #region Lifecycle
        /// <summary>
        /// Initializes the LevelManager, loading the player's current level and setting up the level streaks.
        /// </summary>
        public void OnInit()
        {
            InitLevel();

            SettingManager.OnDisplayNameUpdated += OnDisplayNameUpdated;
        }

        /// <summary>
        /// Disposes of the LevelManager, clearing any data and unsubscribing from events.
        /// </summary>
        public void OnDispose()
        {
            SettingManager.OnDisplayNameUpdated -= OnDisplayNameUpdated;
        }
        #endregion

        #region Level Initialization
        /// <summary>
        /// Initializes the level data, setting the current level and loading any saved streaks from PlayerPrefs.
        /// </summary>
        private void InitLevel()
        {
            UpdateLevelStreakData(CoreGameLevelConfig.MinLevel, 0);

            try
            {
                var key = PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LevelStreakData);

                if (PlayerPrefsUtil.HasKey(key))
                {
                    var json = PlayerPrefsUtil.GetString(key);
                    var levelStreak = JsonConvert.DeserializeObject<LevelStreakData>(json);
                    _currentLevelStreakData = levelStreak;

                    var requiredStreakToPass = CoreGameLevelConfig.GetLevelData(_currentLevelStreakData.level).RequiredDailyWinningStreakToPass;
                    var isLevelPassed = _currentLevelStreakData.streak >= requiredStreakToPass;

                    if (isLevelPassed)
                    {
                        if (_currentLevelStreakData.level < CoreGameLevelConfig.MaxLevel)
                        {
                            UpdateLevelStreakData(_currentLevelStreakData.level + 1, 0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            Logger.Log($"LevelManager::InitLevel - CurrentLevel: {_currentLevelStreakData.level} | CurrentLevelStreak: {_currentLevelStreakData.streak}");
        }
        #endregion

        #region Level Management
        /// <summary>
        /// Gets the current winning streak for the specified level on the current game day.
        /// </summary>
        /// <param name="level">The level to check the winning streak for.</param>
        /// <returns>The current streak for the specified level, or 0 if no streak is found.</returns>
        public int GetLevelWinningStreak(int level)
        {
            if (level == _currentLevelStreakData.level)
            {
                return _currentLevelStreakData.streak;
            }

            return 0;
        }

        /// <summary>
        /// Overrides the current level and resets level winning streak.
        /// </summary>
        /// <param name="level">The new current level.</param>
        public void OverrideCurrentLevel(int level)
        {
            level = Mathf.Clamp(level, CoreGameLevelConfig.MinLevel, CoreGameLevelConfig.MaxLevel);
            UpdateLevelStreakData(level, 0);
            SaveLevelStreakData();

            Logger.Log($"LevelManager::OverrideCurrentLevel - CurrentLevel: {_currentLevelStreakData.level} | CurrentLevelStreak: {_currentLevelStreakData.streak}");
        }

        /// <summary>
        /// Updates the current level streak data
        /// </summary>
        /// <param name="level"></param>
        /// <param name="streak"></param>
        private void UpdateLevelStreakData(int level, int streak)
        {
            _currentLevelStreakData = new LevelStreakData(level, streak);

            Logger.Log($"LevelManager::UpdateLevelStreakData - level: {level} | streak: {streak}");
        }

        /// <summary>
        /// Saves the current level streak data in PlayerPrefs.
        /// </summary>
        private void SaveLevelStreakData()
        {
            var json = JsonConvert.SerializeObject(_currentLevelStreakData);
            PlayerPrefs.SetString(PlayerPrefsUtil.GetUserSpecificKey(PlayerPrefKeys.LevelStreakData), json);
        }

        /// <summary>
        /// Starts a new level and sets up any necessary level data.
        /// </summary>
        /// <param name="level">The level that has started.</param>
        public void OnLevelStarted(int level)
        {
            LatestLevelPlayed = level;
        }

        /// <summary>
        /// Completes a level and updates the player's streak and progress.
        /// </summary>
        /// <param name="level">The level that has been completed.</param>
        /// <param name="isPassed">Whether the level was successfully passed.</param>
        public void OnLevelCompleted(int level, bool isPassed)
        {
            if (level != LatestLevelPlayed)
            {
                Logger.LogError($"Level completed {level} doesn't match the level started {LatestLevelPlayed}!");
                return;
            }

            if (isPassed)
            {
                var levelConfigData = CoreGameLevelConfig.GetLevelData(level);

                if (_currentLevelStreakData.streak + 1 >= levelConfigData.RequiredDailyWinningStreakToPass &&
                    _currentLevelStreakData.level < CoreGameLevelConfig.MaxLevel) // level passed & can level up
                {
                    UpdateLevelStreakData(_currentLevelStreakData.level + 1, 0); // streak reset, level + 1
                }
                else
                {
                    UpdateLevelStreakData(_currentLevelStreakData.level, _currentLevelStreakData.streak + 1); // streak + 1
                }
            }
            else
            {
                UpdateLevelStreakData(_currentLevelStreakData.level, 0); // streak reset
            }

            SaveLevelStreakData();
        }
        #endregion

        #region Event Handling
        /// <summary>
        /// Called when the display name is updated, reinitializing the level data.
        /// </summary>
        private void OnDisplayNameUpdated()
        {
            InitLevel();
        }
        #endregion
    }
}