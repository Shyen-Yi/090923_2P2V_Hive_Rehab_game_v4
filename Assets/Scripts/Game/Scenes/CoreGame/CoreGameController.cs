using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public struct CoreGameData : ISceneData
    {
        public Dictionary<CalibrationStageType, Vector3> playerCornersWorldPos;
        public Dictionary<CalibrationStageType, Vector3> actualCornersWorldPos;
        public Vector3 centerScreenPos;
        public Vector3 spacecraftScreenOffsetFromCursor;

        public CoreGameData(Dictionary<CalibrationStageType, Vector3> playerCornersWorldPos, Dictionary<CalibrationStageType, Vector3> actualCornersWorldPos, Vector3 centerScreenPos, Vector3 spacecraftScreenOffsetFromCursor)
        {
            this.playerCornersWorldPos = playerCornersWorldPos;
            this.actualCornersWorldPos = actualCornersWorldPos;
            this.centerScreenPos = centerScreenPos;
            this.spacecraftScreenOffsetFromCursor = spacecraftScreenOffsetFromCursor;
        }
    }

    public class CoreGameController : GameSceneControllerBase
    {
        #region Fields
        private Dictionary<CalibrationStageType, Vector3> _playerCornersWorldPos;
        private Dictionary<CalibrationStageType, Vector3> _actualCornersWorldPos;
        private Vector3 _centerScreenPos;
        private Vector3 _spacecraftScreenOffsetFromCursor;
        #endregion

        #region Extra
        private enum ExtraConfig
        {
            Net = 0,
            Spacecraft = 1,
        }

        private NetController _netController;
        private SpacecraftController _spacecraftController;
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

            var spacecraftConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.Spacecraft];
            _spacecraftController = new SpacecraftController(spacecraftConfig);
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is CoreGameData pData)
            {
                _playerCornersWorldPos = pData.playerCornersWorldPos;
                _actualCornersWorldPos = pData.actualCornersWorldPos;
                _centerScreenPos = pData.centerScreenPos;
                _spacecraftScreenOffsetFromCursor = pData.spacecraftScreenOffsetFromCursor;
            }

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