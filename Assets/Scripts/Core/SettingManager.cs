using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class SettingManager
    /// @brief Manages game settings, such as the player's display name, total levels, and level goal. Provides functionality for updating and saving settings.
    /// 
    /// The `SettingManager` class handles the user's settings, such as the display name, total level count, and level goal. It loads
    /// these settings from PlayerPrefs, updates them when changed, and saves them back to PlayerPrefs. It also supports resetting the settings
    /// to their default values, and it triggers events when certain settings are updated.
    public class SettingManager : SingletonBase<SettingManager>, ICoreManager
    {
        /// <summary>
        /// Gets whether the user is using the default display name.
        /// </summary>
        public bool IsDefaultUser { get; private set; }

        /// <summary>
        /// Gets or sets the display name of the user. If set to the default value, <see cref="IsDefaultUser"/> is true.
        /// </summary>
        public string DisplayName 
        { 
            get => _displayName; 
            private set
            {
                _displayName = value;
                IsDefaultUser = _displayName == GameGeneralConfig.GetData().DefaultUserName;
            }
        }

        /// <summary>
        /// Gets or sets the total number of asteroids to spawn in each level.
        /// </summary>
        public int LevelTotal { get; private set; }

        /// <summary>
        /// Gets or sets the number of asteroids that the player needs to collect successfully to win a level.
        /// </summary>
        public int LevelGoal { get; private set; }

        private string _displayName;

        #region Events
        /// <summary>
        /// Event triggered when the display name is updated.
        /// </summary>
        public static Action OnDisplayNameUpdated;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Initializes the `SettingManager` by loading the user's settings from PlayerPrefs.
        /// </summary>
        public void OnInit()
        {
            var displayName = PlayerPrefsUtil.HasKey(PlayerPrefKeys.DisplayName) ? PlayerPrefsUtil.GetString(PlayerPrefKeys.DisplayName) : GameGeneralConfig.GetData().DefaultUserName;
            UpdateDisplayName(displayName, false);

            var levelTotal = PlayerPrefsUtil.HasKey(PlayerPrefKeys.LevelTotal) ? PlayerPrefsUtil.GetInt(PlayerPrefKeys.LevelTotal) : GameGeneralConfig.GetData().DefaultBlock;
            UpdateLevelTotal(levelTotal, false);

            var levelGoal = PlayerPrefsUtil.HasKey(PlayerPrefKeys.LevelGoal) ? PlayerPrefsUtil.GetInt(PlayerPrefKeys.LevelGoal) : GameGeneralConfig.GetData().DefaultGoal;
            UpdateLevelGoal(levelGoal, false);

            Logger.Log($"Setting loaded. Username: {DisplayName} | LevelTotal: {LevelTotal} | LevelGoal: {LevelGoal}");
        }

        /// <summary>
        /// Disposes of the `SettingManager` by saving settings like display name and level total.
        /// </summary>
        public void OnDispose()
        {
            SaveDisplayName();
            SaveLevelTotal();
        }
        #endregion

        #region Settings Management
        /// <summary>
        /// Resets the settings to their default values.
        /// </summary>
        public void Reset()
        {
            if (PlayerPrefsUtil.HasKey(PlayerPrefKeys.DisplayName))
            {
                PlayerPrefsUtil.TryDeleteKey(PlayerPrefKeys.DisplayName);
            }

            if (PlayerPrefsUtil.HasKey(PlayerPrefKeys.LevelTotal))
            {
                PlayerPrefsUtil.TryDeleteKey(PlayerPrefKeys.LevelTotal);
            }

            if (PlayerPrefsUtil.HasKey(PlayerPrefKeys.LevelGoal))
            {
                PlayerPrefsUtil.TryDeleteKey(PlayerPrefKeys.LevelGoal);
            }

            DisplayName = GameGeneralConfig.GetData().DefaultUserName;
            LevelTotal = GameGeneralConfig.GetData().DefaultBlock;
            LevelGoal = GameGeneralConfig.GetData().DefaultGoal;
        }

        /// <summary>
        /// Saves the user's display name to PlayerPrefs.
        /// </summary>
        private void SaveDisplayName()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefsUtil.TrySetString(PlayerPrefKeys.DisplayName, DisplayName);
                Logger.Log($"Username {DisplayName} saved.");
            }
        }

        /// <summary>
        /// Saves the LevelTotal (the total number of asteroids to spawn in each level) to PlayerPrefs.
        /// </summary>
        private void SaveLevelTotal()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefsUtil.TrySetInt(PlayerPrefKeys.LevelTotal, LevelTotal);
                Logger.Log($"SettingManager::SaveLevelTotal - LevelTotal {LevelTotal} saved.");
            }
        }

        /// <summary>
        /// Saves the LevelGoal (the number of asteroids that the user needs to collect successfully to win a level) to PlayerPrefs.
        /// </summary>
        private void SaveLevelGoal()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefsUtil.TrySetInt(PlayerPrefKeys.LevelGoal, LevelGoal);
                Logger.Log($"SettingManager::SaveLevelGoal - LevelGoal {LevelGoal} saved.");
            }
        }

        /// <summary>
        /// Updates the display name of the user and triggers the `OnDisplayNameUpdated` event if saved.
        /// </summary>
        /// <param name="displayName">The new display name for the user.</param>
        /// <param name="toSave">Whether to save the display name to PlayerPrefs.</param>
        public void UpdateDisplayName(string displayName, bool toSave)
        {
            DisplayName = displayName;

            if (toSave)
            {
                SaveDisplayName();
            }

            OnDisplayNameUpdated?.Invoke();
        }

        /// <summary>
        /// Updates the LevelTotal (the total number of asteroids to spawn in each level) and saves it if necessary.
        /// </summary>
        /// <param name="levelTotal">LevelTotal (the total number of asteroids to spawn in each level)</param>
        /// <param name="toSave">Whether to save the new LevelTotal (the total number of asteroids to spawn in each level) to PlayerPrefs.</param>
        public void UpdateLevelTotal(int levelTotal, bool toSave)
        {
            LevelTotal = levelTotal;

            if (toSave)
            {
                SaveLevelTotal();
            }
        }

        /// <summary>
        /// Updates LevelGoal (the number of asteroids that the user needs to collect successfully to win a level) and saves it if necessary.
        /// </summary>
        /// <param name="levelGoal">The LevelGoal (the number of asteroids that the user needs to collect successfully to win a level) to update to.</param>
        /// <param name="toSave">Whether to save the LevelGoal (the number of asteroids that the user needs to collect successfully to win a level) to PlayerPrefs.</param>
        public void UpdateLevelGoal(int levelGoal, bool toSave)
        {
            LevelGoal = levelGoal;

            if (toSave)
            {
                SaveLevelGoal();
            }
        }
        #endregion
    }
}