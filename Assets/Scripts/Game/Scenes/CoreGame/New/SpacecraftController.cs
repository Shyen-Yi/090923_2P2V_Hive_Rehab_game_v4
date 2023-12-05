using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class SpacecraftController
    {
        #region Extra
        private enum ExtraRenderer
        {
            FireLeft = 0,
            FireRight = 1,
            Idle = 2,
            Moving = 3,
            Capturing = 4,
        }

        private SpriteRenderer _fireLeftRenderer;
        private SpriteRenderer _fireRightRenderer;
        private SpriteRenderer _idleRenderer;
        private SpriteRenderer _movingRenderer;
        private SpriteRenderer _capturingRenderer;
        #endregion

        private GeneralWidgetConfig _config;
        private bool _isMoving;
        private bool _isCapturing;
        private bool _isActive;
        private float _movingVal;
        private float _capturingVal;

        private static readonly float MovingValIncreaseSec = .35f;
        private static readonly float MovingValDecreaseSec = .5f;
        private static readonly float CapturingValIncreaseSec = .3f;
        private static readonly float CapturingValDecreaseSec = .3f;

        public SpacecraftController(GeneralWidgetConfig config)
        {
            _config = config;
            InitExtra();

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void Dispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }

        private void InitExtra()
        {
            _fireLeftRenderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.FireLeft];
            _fireRightRenderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.FireRight];
            _idleRenderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Idle];
            _movingRenderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Moving];
            _capturingRenderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Capturing];
        }

        public void Activate()
        {
            _config.gameObject.SetActive(true);
            _isActive = true;
        }

        public void Deactivate()
        {
            _config.gameObject.SetActive(false);
            _isActive = false;

            Reset();
        }

        private void Reset()
        {
            _isMoving = false;
            _movingVal = 0;
            _capturingVal = 0;
            SetCapturing(false);
        }

        public Vector3 GetWorldPos()
        {
            return _config.transform.position;
        }

        public void SetWorldPos(Vector3 worldPos)
        {
            _config.transform.position = worldPos;
        }

        public void SetMoving(bool isMoving)
        {
            _isMoving = isMoving;
        }

        public void SetCapturing(bool isCapturing)
        {
            _isCapturing = isCapturing;
        }

        private void Tick()
        {
            if (!_isActive)
                return;

            var movingDirection = _isMoving ? 1 : -1;
            var movingRate = _isMoving ? MovingValIncreaseSec : MovingValDecreaseSec;
            var movingVal = Mathf.Clamp01(_movingVal + movingDirection / movingRate * Time.deltaTime);
            _movingVal = movingVal;

            var capturingDirection = _isCapturing ? 1 : -1;
            var capturingRate = _isCapturing ? CapturingValIncreaseSec : CapturingValDecreaseSec;
            var capturingVal = Mathf.Clamp01(_capturingVal + capturingDirection / capturingRate * Time.deltaTime);
            _capturingVal = capturingVal;

            PostTick();
        }

        private void PostTick()
        {
            var color = _fireLeftRenderer.color;
            _fireLeftRenderer.color = _fireRightRenderer.color = new Color(color.r, color.g, color.b, _movingVal);

            var idleColor = _idleRenderer.color;
            _idleRenderer.color = new Color(idleColor.r, idleColor.g, idleColor.b, 1 - _movingVal);

            var movingColor = _movingRenderer.color;
            _movingRenderer.color = new Color(movingColor.r, movingColor.g, movingColor.b, _movingVal);

            var capturingColor = _capturingRenderer.color;
            _capturingRenderer.color = new Color(capturingColor.r, capturingColor.g, capturingColor.b, _capturingVal);
        }
    }
}