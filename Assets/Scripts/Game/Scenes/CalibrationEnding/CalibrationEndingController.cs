using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public struct CalibrationEndingData : ISceneData
    {
        public CoreGameData coreGameData;

        public CalibrationEndingData(CoreGameData coreGameData)
        {
            this.coreGameData = coreGameData;
        }
    }

    public class CalibrationEndingController : GameSceneControllerBase
    {
        #region Fields
        private CoreGameData _coreGameData;
        #endregion

        #region Extra
        private HiveButton _exitButton;
        private HiveButton _redoButton;
        private HiveButton _playButton;

        private enum ExtraBtn
        {
            Exit = 0,
            Redo = 1,
            Play = 2,
        }
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            base.OnInit();

            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _exitButton = Config.ExtraButtons[(int)ExtraBtn.Exit];
            _redoButton = Config.ExtraButtons[(int)ExtraBtn.Redo];
            _playButton = Config.ExtraButtons[(int)ExtraBtn.Play];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is CalibrationEndingData pData)
            {
                _coreGameData = pData.coreGameData;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _exitButton.onClick.AddListener(OnExitButtonClick);
            _redoButton.onClick.AddListener(OnRedoButtonClick);
            _playButton.onClick.AddListener(OnPlayButtonClick);
        }

        private void UnbindActions()
        {
            _exitButton.onClick.RemoveAllListeners();
            _redoButton.onClick.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnExitButtonClick()
        {
            GameSceneManager.Instance.GoBack(SceneNames.MainMenu);
        }

        private void OnRedoButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.Calibration, null, () =>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }

        private void OnPlayButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.TransitionCoreGame, new TransitionCoreGameData(_coreGameData), () =>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }
        #endregion
    }
}