using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class SettingManager : SingletonBase<SettingManager>, ICoreManager
    {
        public string DisplayName { get; private set; }
        public int Level { get; private set; }
        public int DailyBlock { get; private set; }

        #region Lifecycle
        public void OnInit()
        { 
            DisplayName = PlayerPrefs.HasKey(PlayerPrefKeys.DisplayName) ? PlayerPrefs.GetString(PlayerPrefKeys.DisplayName) : GameGeneralConfig.GetData().DefaultUserName;
            Level = PlayerPrefs.HasKey(PlayerPrefKeys.Level) ? PlayerPrefs.GetInt(PlayerPrefKeys.Level) : CoreGameLevelConfig.MinLevel;
            DailyBlock = PlayerPrefs.HasKey(PlayerPrefKeys.DailyBlock) ? PlayerPrefs.GetInt(PlayerPrefKeys.DailyBlock) : GameGeneralConfig.GetData().DefaultGoal;
        }

        public void OnDispose()
        {
            SaveDisplayName();
            SaveLevel();
            SaveDailyBlock();
        }
        #endregion

        private void SaveDisplayName()
        {
            PlayerPrefs.SetString(PlayerPrefKeys.DisplayName, DisplayName);
        }

        private void SaveLevel()
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.Level, Level);
        }

        private void SaveDailyBlock()
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.DailyBlock, DailyBlock);
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