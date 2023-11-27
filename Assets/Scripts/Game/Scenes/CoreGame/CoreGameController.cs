using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class CoreGameController : GameSceneControllerBase
    {
        #region Extra
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            InputManager.Instance.HideCursor();
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            InputManager.Instance.ShowCursor();
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
        }

        private void UnbindActions()
        {
        }
        #endregion

        #region Callback
        #endregion
    }
}