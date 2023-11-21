using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class StatsMenuController : GameSceneControllerBase
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
        }

        private void UnbindActions()
        {
        }
        #endregion

        #region Callback
        #endregion
    }
}