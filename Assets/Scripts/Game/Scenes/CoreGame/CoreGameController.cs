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

        public CoreGameData(Dictionary<CalibrationStageType, Vector3> playerCornersWorldPos, Dictionary<CalibrationStageType, Vector3> actualCornersWorldPos, Vector3 centerScreenPos)
        {
            this.playerCornersWorldPos = playerCornersWorldPos;
            this.actualCornersWorldPos = actualCornersWorldPos;
            this.centerScreenPos = centerScreenPos;
        }
    }

    public class CoreGameController : GameSceneControllerBase
    {
        #region Fields
        private Dictionary<CalibrationStageType, Vector3> _playerCornersWorldPos;
        private Dictionary<CalibrationStageType, Vector3> _actualCornersWorldPos;
        private Vector3 _centerScreenPos;
        private Vector3 _spacecraftScreenOffsetFromCursor;
        private Vector3 _spacecraftScreenPos;
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
            }

            CenterSpacecraft();

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
            MonoBehaviourUtil.OnUpdate += Tick;
        }

        private void UnbindActions()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Callback
        private void Tick()
        {
            var spacecraftScreenPos = GetSpacecraftScreenPos();
            var spacecraftSpeed = (_spacecraftScreenPos - spacecraftScreenPos).sqrMagnitude / Time.deltaTime;
            _spacecraftScreenPos = spacecraftScreenPos;
            _spacecraftController.SetMoving(spacecraftSpeed > .1f);

            var spacecraftWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(spacecraftScreenPos);
            _spacecraftController.SetWorldPos(new Vector3(spacecraftWorldPos.x, spacecraftWorldPos.y, _spacecraftController.GetWorldPos().z));
        }
        #endregion

        #region Content
        private Vector3 GetSpacecraftScreenPosRaw()
        {
            return InputManager.Instance.CursorScreenPos + _spacecraftScreenOffsetFromCursor;
        }

        private Vector3 GetSpacecraftScreenPos()
        {
            var screenPos = GetSpacecraftScreenPosRaw();

            var section = CalibrationStageType.Center;

            if (screenPos.x <= _centerScreenPos.x && screenPos.y >= _centerScreenPos.y)
            {
                section = CalibrationStageType.TopLeft;
            }
            else if (screenPos.x >= _centerScreenPos.x && screenPos.y >= _centerScreenPos.y)
            {
                section = CalibrationStageType.TopRight;
            }
            else if (screenPos.x >= _centerScreenPos.x && screenPos.y <= _centerScreenPos.y)
            {
                section = CalibrationStageType.BottomRight;
            }
            else if (screenPos.x <= _centerScreenPos.x && screenPos.y <= _centerScreenPos.y)
            {
                section = CalibrationStageType.BottomLeft;
            }

            if (_actualCornersWorldPos.TryGetValue(section, out var actualCornerWorldPos) &&
                _playerCornersWorldPos.TryGetValue(section, out var playerCornerWorldPos))
            {
                var actualCornerScreenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(actualCornerWorldPos);
                var playerCornerScreenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(playerCornerWorldPos);

                actualCornerScreenPos = new Vector3(actualCornerScreenPos.x - _centerScreenPos.x, actualCornerScreenPos.y - _centerScreenPos.y, actualCornerScreenPos.z);
                playerCornerScreenPos = new Vector3(playerCornerScreenPos.x - _centerScreenPos.x, playerCornerScreenPos.y - _centerScreenPos.y, playerCornerScreenPos.z);

                var scaleVector = new Vector2(actualCornerScreenPos.x / playerCornerScreenPos.x, actualCornerScreenPos.y / playerCornerScreenPos.y);

                screenPos = new Vector3(screenPos.x - _centerScreenPos.x, screenPos.y - _centerScreenPos.y, screenPos.z);
                screenPos = new Vector3(screenPos.x * scaleVector.x, screenPos.y * scaleVector.y, screenPos.z);
                screenPos = new Vector3(screenPos.x + _centerScreenPos.x, screenPos.y + _centerScreenPos.y, screenPos.z);
            }

            return screenPos;
        }

        private void CenterSpacecraft()
        {
            _spacecraftScreenOffsetFromCursor = _centerScreenPos - InputManager.Instance.CursorScreenPos;
        }
        #endregion
    }
}