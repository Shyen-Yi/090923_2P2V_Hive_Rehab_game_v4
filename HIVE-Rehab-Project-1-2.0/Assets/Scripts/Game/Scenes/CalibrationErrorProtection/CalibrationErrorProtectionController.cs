using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class CalibrationErrorProtectionController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Back = 0,
            Contact = 1,
            Question = 2,
        }

        private HiveButton _backButton;
        private HiveButton _contactButton;
        private HiveButton _questionButton;

        private void InitExtra()
        {
            _backButton = Config.ExtraButtons[(int)ExtraBtn.Back];
            _contactButton = Config.ExtraButtons[(int)ExtraBtn.Contact];
            _questionButton = Config.ExtraButtons[(int)ExtraBtn.Question];
        }
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            _backButton.onClick.AddListener(OnBackButtonClick);
            _contactButton.onClick.AddListener(OnContactButtonClick);
            _questionButton.onClick.AddListener(OnQuestionButtonClick);
        }

        private void UnbindActions()
        {
            _backButton.onClick.RemoveAllListeners();
            _contactButton.onClick.RemoveAllListeners();
            _questionButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnBackButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.GoBack();
        }

        private void OnContactButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Calibration));
        }
        #endregion
    }
}