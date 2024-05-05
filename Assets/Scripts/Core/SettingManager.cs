using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class SettingManager : SingletonBase<SettingManager>, ICoreManager
    {
        public bool IsDefaultUser { get; private set; }
        public string DisplayName 
        { 
            get => _displayName; 
            private set
            {
                _displayName = value;
                IsDefaultUser = _displayName == GameGeneralConfig.GetData().DefaultUserName;
            }
        }
        public int Level { get; private set; }
        public int DailyBlock { get; private set; }

        private string _displayName;

        #region Lifecycle
        public void OnInit()
        {
            DisplayName = PlayerPrefs.HasKey(PlayerPrefKeys.DisplayName) ? PlayerPrefs.GetString(PlayerPrefKeys.DisplayName) : GameGeneralConfig.GetData().DefaultUserName;
            Level = PlayerPrefs.HasKey(PlayerPrefKeys.Level) ? PlayerPrefs.GetInt(PlayerPrefKeys.Level) : CoreGameLevelConfig.MinLevel;
            DailyBlock = PlayerPrefs.HasKey(PlayerPrefKeys.DailyBlock) ? PlayerPrefs.GetInt(PlayerPrefKeys.DailyBlock) : GameGeneralConfig.GetData().DefaultGoal;

            Logger.Log($"Setting loaded. Username: {DisplayName} | Level: {Level} | DailyBlock: {DailyBlock}");
        }

        public void OnDispose()
        {
            SaveDisplayName();
            SaveLevel();
            SaveDailyBlock();
        }
        #endregion

        public void Reset()
        {
            if (PlayerPrefs.HasKey(PlayerPrefKeys.DisplayName))
            {
                PlayerPrefs.DeleteKey(PlayerPrefKeys.DisplayName);
            }

            if (PlayerPrefs.HasKey(PlayerPrefKeys.Level))
            {
                PlayerPrefs.DeleteKey(PlayerPrefKeys.Level);
            }

            if (PlayerPrefs.HasKey(PlayerPrefKeys.DailyBlock))
            {
                PlayerPrefs.DeleteKey(PlayerPrefKeys.DailyBlock);
            }

            DisplayName = GameGeneralConfig.GetData().DefaultUserName;
            Level = CoreGameLevelConfig.MinLevel;
            DailyBlock = GameGeneralConfig.GetData().DefaultGoal;
        }

        private void SaveDisplayName()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefs.SetString(PlayerPrefKeys.DisplayName, DisplayName);
                Logger.Log($"Username {DisplayName} saved.");
            }
        }

        private void SaveLevel()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefs.SetInt(PlayerPrefKeys.Level, Level);
                Logger.Log($"Level {Level} saved.");
            }
        }

        private void SaveDailyBlock()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefs.SetInt(PlayerPrefKeys.DailyBlock, DailyBlock);
                Logger.Log($"DailyBlock {DailyBlock} saved.");
            }
        }

        public void UpdateDisplayName(string displayName, bool toSave)
        {
            DisplayName = displayName;

            if (toSave)
            {
                SaveDisplayName();
            }
        }

        public void UpdateLevel(int level, bool toSave)
        {
            Level = level;

            if (toSave)
            {
                SaveLevel();
            }
        }

        public void UpdateDailyBlock(int block, bool toSave)
        {
            DailyBlock = block;

            if (toSave)
            {
                SaveDailyBlock();
            }
        }
    }
}