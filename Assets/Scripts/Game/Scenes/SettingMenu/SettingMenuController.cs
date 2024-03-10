using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    public class SettingMenuController : GameSceneControllerBase
    {
        private enum SettingMenuWidgetType
        {
            None,
            Settings,
            Unlock
        }

        #region Extra
        private enum ExtraConfig
        {
            SettingsWidget = 0,
            UnlockWidget = 1,
        }

        private enum ExtraBtn
        {
            Cross = 0,
            Mail = 1,
            Question = 2,
            Lock = 3,
            Unlock = 4,
        }

        private SettingMenuSettingsWidgetController _settingsWidgetController;
        private SettingMenuUnlockWidgetController _unlockWidgetController;

        private HiveButton _crossButton;
        private HiveButton _mailButton;
        private HiveButton _questionButton;
        private HiveButton _lockButton;
        private HiveButton _unlockButton;
        #endregion

        #region Fields
        private SettingMenuWidgetType _widgetType = SettingMenuWidgetType.None;

        private static readonly SettingMenuWidgetType DefaultWidget = SettingMenuWidgetType.Unlock;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            var settingsWidgetConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.SettingsWidget];
            _settingsWidgetController = new SettingMenuSettingsWidgetController(settingsWidgetConfig);
            _settingsWidgetController.Init();
            var unlockWidgetConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.UnlockWidget];
            _unlockWidgetController = new SettingMenuUnlockWidgetController(unlockWidgetConfig);
            _unlockWidgetController.Init();

            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];
            _mailButton = Config.ExtraButtons[(int)ExtraBtn.Mail];
            _questionButton = Config.ExtraButtons[(int)ExtraBtn.Question];
            _lockButton = Config.ExtraButtons[(int)ExtraBtn.Lock];
            _unlockButton = Config.ExtraButtons[(int)ExtraBtn.Unlock];
        }

        private void DisposeExtra()
        {
            _settingsWidgetController.Dispose();
            _unlockWidgetController.Dispose();
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (showState == GameSceneShowState.New)
            {
                _widgetType = DefaultWidget;
            }

            ShowWidget(_widgetType, true);
        }

        protected override void OnDispose()
        {
            DisposeExtra();
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _crossButton.onClick.AddListener(OnCrossButtonClick);
            _mailButton.onClick.AddListener(OnMailButtonClick);
            _questionButton.onClick.AddListener(OnQuestionButtonClick);
            _lockButton.onClick.AddListener(OnLockButtonClick);
            _unlockButton.onClick.AddListener(OnUnlockButtonClick);
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
            _mailButton.onClick.RemoveAllListeners();
            _questionButton.onClick.RemoveAllListeners();
            _lockButton.onClick.RemoveAllListeners();
            _unlockButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Content
        private void ShowWidget(SettingMenuWidgetType type, bool forceUpdate = false)
        {
            if (_widgetType == type && !forceUpdate)
                return;

            _widgetType = type;

            if (_widgetType == SettingMenuWidgetType.Settings)
            {
                _settingsWidgetController.Show();
            }
            else
            {
                _settingsWidgetController.Hide();
            }

            if (_widgetType == SettingMenuWidgetType.Unlock)
            {
                _unlockWidgetController.Show();
            }
            else
            {
                _unlockWidgetController.Hide();
            }

            _unlockButton.gameObject.SetActive(_widgetType == SettingMenuWidgetType.Unlock);
            _lockButton.gameObject.SetActive(_widgetType != SettingMenuWidgetType.Unlock);
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.GoBack();
        }

        private void OnMailButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Setting));
        }

        private void OnLockButtonClick()
        {
            ShowWidget(SettingMenuWidgetType.Unlock);
        }

        private void OnUnlockButtonClick()
        {
            var canUnlock = true;
            if (canUnlock)
            {
                ShowWidget(SettingMenuWidgetType.Settings);
            }
            else
            {

            }
        }
        #endregion
    }
}