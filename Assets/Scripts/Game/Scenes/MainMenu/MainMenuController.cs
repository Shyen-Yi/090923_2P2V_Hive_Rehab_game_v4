using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
            Quit = 3,
        }

        private HiveButton _startButton;
        private HiveButton _settingsButton;
        private HiveButton _statsButton;
        private HiveButton _quitButton;
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
            _quitButton = Config.ExtraButtons[(int)ExtraBtn.Quit];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            SoundManager.Instance.StopSound(SoundType.CalibrationBackground);
            SoundManager.Instance.StopSound(SoundType.CoreGameBackground);
            SoundManager.Instance.StopSound(SoundType.CoreGameLevelEnd);
            SoundManager.Instance.StopSound(SoundType.CoreGameLevelEndShowReport);
            SoundManager.Instance.PlaySound(SoundType.MenuBackground);

            base.OnShow(data, showState);
        }

        protected override void OnDispose()
        {
            UnbindActions();

            base.OnDispose();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _statsButton.onClick.AddListener(OnStatsButtonClick);
            _quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        private void UnbindActions()
        {
            _startButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _statsButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnStartButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            if (DebugConfig.GetData().EnableCalibration) 
            {
                GameSceneManager.Instance.ShowScene(SceneNames.TransitionCalibration);
            }
            else
            {
                GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, new CoreGameData(
                    new Vector3(0, 0),
                    new Vector3(Screen.width, Screen.height),
                    new Vector3(Screen.width / 2, Screen.height / 2),
                    1,
                    SettingManager.Instance.Level));

                //GameSceneManager.Instance.ShowScene(SceneNames.TransitionCoreGame, new CoreGameData(
                //    new Vector3(0, 0),
                //    new Vector3(Screen.width, Screen.height),
                //    new Vector3(Screen.width / 2, Screen.height / 2),
                //    1,
                //    SettingManager.Instance.Level));
            }
        }

        private void OnSettingsButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.SettingMenu);
        }

        private void OnStatsButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.StatsMenu);
        }

        private void OnQuitButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameManager.Instance.QuitGame();
        }
        #endregion
    }
}