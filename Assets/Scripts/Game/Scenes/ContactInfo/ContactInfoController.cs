using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.hive.projectr
{
    public class ContactInfoController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Cross = 0,
        }

        private HiveButton _crossButton;
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
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(SceneName);
        }
        #endregion
    }
}