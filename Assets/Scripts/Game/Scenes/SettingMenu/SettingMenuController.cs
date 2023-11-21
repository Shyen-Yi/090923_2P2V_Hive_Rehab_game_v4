using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class SettingMenuController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Cross = 0,
            Mail = 1,
            Question = 2,
        }

        private HiveButton _crossButton;
        private HiveButton _mailButton;
        private HiveButton _questionButton;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];
            _mailButton = Config.ExtraButtons[(int)ExtraBtn.Mail];
            _questionButton = Config.ExtraButtons[(int)ExtraBtn.Question];
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
            _crossButton.onClick.AddListener(OnCrossButtonClick);
            _mailButton.onClick.AddListener(OnMailButtonClick);
            _questionButton.onClick.AddListener(OnQuestionButtonClick);
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
            _mailButton.onClick.RemoveAllListeners();
            _questionButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(Name);
        }

        private void OnMailButtonClick()
        {
            LogHelper.LogError($"Mail");
        }

        private void OnQuestionButtonClick()
        {
            LogHelper.LogError($"Question");
        }
        #endregion
    }
}