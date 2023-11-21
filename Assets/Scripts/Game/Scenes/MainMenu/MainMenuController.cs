using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class MainMenuController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Start = 0,
            Settings = 1,
            Stats = 2,
        }

        private HiveButton _startButton;
        private HiveButton _settingsButton;
        private HiveButton _statsButton;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _startButton = Config.ExtraButtons[(int)ExtraBtn.Start];
            _settingsButton = Config.ExtraButtons[(int)ExtraBtn.Settings];
            _statsButton = Config.ExtraButtons[(int)ExtraBtn.Stats];
        }

        protected override void OnShow(ISceneData data)
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _statsButton.onClick.AddListener(OnStatsButtonClick);
        }

        private void UnbindActions()
        {
            _startButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _statsButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnStartButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.TransitionCoreGame);
        }

        private void OnSettingsButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.SettingMenu);
        }

        private void OnStatsButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.StatsMenu);
        }
        #endregion
    }
}