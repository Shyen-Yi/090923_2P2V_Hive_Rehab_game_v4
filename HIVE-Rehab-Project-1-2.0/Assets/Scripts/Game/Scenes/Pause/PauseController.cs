using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public struct PauseData : ISceneData
    {
        public Action onResume;

        public PauseData(Action onResume)
        {
            this.onResume = onResume;
        }
    }

    public class PauseController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Resume = 0,
        }

        private HiveButton _resumeButton;
        #endregion

        #region Fields
        private Action _onResume;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _resumeButton = Config.ExtraButtons[(int)ExtraBtn.Resume];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is PauseData pData)
            {
                _onResume = pData.onResume;
            }

            InputManager.Instance.ShowCursor();
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            if (hideState == GameSceneHideState.Removed)
            {
                _onResume?.Invoke();
            }

            InputManager.Instance.HideCursor();
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClick);
        }

        private void UnbindActions()
        {
            _resumeButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Event
        private void OnResumeButtonClick()
        {
            GameSceneManager.Instance.GoBack();
        }
        #endregion
    }
}