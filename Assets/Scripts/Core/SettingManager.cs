using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        public int LevelTotal { get; private set; }
        public int LevelGoal { get; private set; }

        private string _displayName;

        #region Events
        public static Action OnDisplayNameUpdated;
        #endregion

        #region Lifecycle
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

        public void OnDispose()
        {
            SaveDisplayName();
            SaveLevelTotal();
        }
        #endregion

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

        private void SaveDisplayName()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefsUtil.TrySetString(PlayerPrefKeys.DisplayName, DisplayName);
                Logger.Log($"Username {DisplayName} saved.");
            }
        }

        private void SaveLevelTotal()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefsUtil.TrySetInt(PlayerPrefKeys.LevelTotal, LevelTotal);
                Logger.Log($"LevelTotal {LevelTotal} saved.");
            }
        }

        private void SaveLevelGoal()
        {
            if (!IsDefaultUser)
            {
                PlayerPrefsUtil.TrySetInt(PlayerPrefKeys.LevelGoal, LevelGoal);
                Logger.Log($"LevelGoal {LevelGoal} saved.");
            }
        }

        public void UpdateDisplayName(string displayName, bool toSave)
        {
            DisplayName = displayName;

            if (toSave)
            {
                SaveDisplayName();
            }

            OnDisplayNameUpdated?.Invoke();
        }

        public void UpdateLevelTotal(int levelTotal, bool toSave)
        {
            LevelTotal = levelTotal;

            if (toSave)
            {
                SaveLevelTotal();
            }
        }

        public void UpdateLevelGoal(int levelGoal, bool toSave)
        {
            LevelGoal = levelGoal;

            if (toSave)
            {
                SaveLevelGoal();
            }
        }
    }
}