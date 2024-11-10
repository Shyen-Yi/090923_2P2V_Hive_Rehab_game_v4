using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class DailyMaxSessionReachedController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Exit = 0,
            Confirm = 1,
        }

        private HiveButton _exitButton;
        private HiveButton _confirmButton;
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
            _confirmButton = Config.ExtraButtons[(int)ExtraBtn.Confirm];
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
            _confirmButton.onClick.AddListener(OnConfirmButtonClick);
        }

        private void UnbindActions()
        {
            _exitButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Events
        private void OnExitButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            GameSceneManager.Instance.GoBack(SceneNames.MainMenu);
        }

        private void OnConfirmButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            GameSceneManager.Instance.GoBack(SceneNames.MainMenu);

        }
        #endregion
    }
}