using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public struct CoreGameData : ISceneData
    {
        public float spacecraftMovementScale;
        public Vector3 centerScreenPos;

        public CoreGameData(float spacecraftMovementScale, Vector3 centerScreenPos)
        {
            this.spacecraftMovementScale = spacecraftMovementScale;
            this.centerScreenPos = centerScreenPos;
        }
    }

    public class CoreGameController : GameSceneControllerBase
    {
        #region Fields
        private float _spacecraftMovementScale;
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
                _spacecraftMovementScale = pData.spacecraftMovementScale;
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

            screenPos = new Vector3(screenPos.x - _centerScreenPos.x, screenPos.y - _centerScreenPos.y, screenPos.z);
            screenPos = new Vector3(screenPos.x * _spacecraftMovementScale, screenPos.y * _spacecraftMovementScale, screenPos.z);
            screenPos = new Vector3(screenPos.x + _centerScreenPos.x, screenPos.y + _centerScreenPos.y, screenPos.z);

            return screenPos;
        }

        private void CenterSpacecraft()
        {
            _spacecraftScreenOffsetFromCursor = _centerScreenPos - InputManager.Instance.CursorScreenPos;
        }
        #endregion
    }
}