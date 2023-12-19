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

        private static readonly string PlayerPrefKeyDisplayName = "DisplayName";
        private static readonly string PlayerPrefKeyLevel = "Level";
        private static readonly string PlayerPrefKeyDailyBlock = "DailyBlock";

        #region Lifecycle
        public void OnInit()
        { 
            DisplayName = PlayerPrefs.HasKey(PlayerPrefKeyDisplayName) ? PlayerPrefs.GetString(PlayerPrefKeyDisplayName) : "";
            Level = PlayerPrefs.HasKey(PlayerPrefKeyLevel) ? PlayerPrefs.GetInt(PlayerPrefKeyLevel) : CoreGameLevelConfig.MinLevel;
            DailyBlock = PlayerPrefs.HasKey(PlayerPrefKeyDailyBlock) ? PlayerPrefs.GetInt(PlayerPrefKeyDailyBlock) : GameGeneralConfig.GetData().DefaultGoal;
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
            PlayerPrefs.SetString(PlayerPrefKeyDisplayName, DisplayName);
        }

        private void SaveLevel()
        {
            PlayerPrefs.SetInt(PlayerPrefKeyLevel, Level);
        }

        private void SaveDailyBlock()
        {
            PlayerPrefs.SetInt(PlayerPrefKeyDailyBlock, DailyBlock);
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