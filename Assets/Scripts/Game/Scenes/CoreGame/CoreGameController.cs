using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class CoreGameController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraConfig
        {
            Net = 0,
        }

        private NetController _netController;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            var netConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.Net];
            _netController = new NetController(netConfig);
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            InputManager.HideCursor();
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            InputManager.ShowCursor();
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