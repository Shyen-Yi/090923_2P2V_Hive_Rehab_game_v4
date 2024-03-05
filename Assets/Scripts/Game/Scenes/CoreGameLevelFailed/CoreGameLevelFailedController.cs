using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public struct CoreGameLevelFailedData : ISceneData
    {
        public CoreGameData coreGameData;

        public CoreGameLevelFailedData(CoreGameData coreGameData)
        {
            this.coreGameData = coreGameData;
        }
    }

    public class CoreGameLevelFailedController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Exit = 0,
            Replay = 1,
        }

        private HiveButton _exitButton;
        private HiveButton _replayButton;
        #endregion

        #region Fields
        private CoreGameData _coreGameData;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _exitButton = Config.ExtraButtons[(int)ExtraBtn.Exit];
            _replayButton = Config.ExtraButtons[(int)ExtraBtn.Replay];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is CoreGameLevelFailedData pData)
            {
                _coreGameData = pData.coreGameData;
            }

            SoundManager.Instance.PlaySound(SoundType.CoreGameLevelEndShowReport);
            SoundManager.Instance.PlaySound(SoundType.MenuBackground);
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            SoundManager.Instance.StopSound(SoundType.MenuBackground);
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _exitButton.onClick.AddListener(OnExitButtonClick);
            _replayButton.onClick.AddListener(OnReplayButtonClick);
        }

        private void UnbindActions()
        {
            _exitButton.onClick.RemoveAllListeners();
            _replayButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnExitButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.GoBack(SceneNames.MainMenu);
        }

        private void OnReplayButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, _coreGameData, () =>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }
        #endregion
    }
}