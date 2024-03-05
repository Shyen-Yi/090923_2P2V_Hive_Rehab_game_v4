using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace com.hive.projectr
{
    public struct SpacecraftData
    {
        public float size;
        public SpacecraftData(float size)
        {
            this.size = size;
        }
    }

    public class SpacecraftController
    {
        public Transform Transform => _config.transform;

        protected SpacecraftConfig _config;
        private bool _isMoving;
        private bool _isCapturing;
        private bool _isActive;
        private float _movingVal;
        private float _capturingVal;

        private static readonly float MovingValIncreaseSec = .35f;
        private static readonly float MovingValDecreaseSec = .5f;
        private static readonly float CapturingValIncreaseSec = .3f;
        private static readonly float CapturingValDecreaseSec = .3f;

        public SpacecraftController(SpacecraftConfig config)
        {
            _config = config;

            RefreshTick();

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void Dispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }

        public void Activate(SpacecraftData data)
        {
            _config.transform.localScale = Vector3.one * data.size;

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

        public float GetWorldSpaceCaptureRadius()
        {
            return _config.CircleCollider.radius * _config.transform.lossyScale.x;
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

            RefreshTick();
            SoundTick();
        }

        private void RefreshTick()
        {
            var color = _config.FireLeftRenderer.color;
            _config.FireLeftRenderer.color = _config.FireRightRenderer.color = new Color(color.r, color.g, color.b, _movingVal);

            var idleColor = _config.IdleRenderer.color;
            _config.IdleRenderer.color = new Color(idleColor.r, idleColor.g, idleColor.b, 1 - _movingVal);

            var movingColor = _config.MovingRenderer.color;
            _config.MovingRenderer.color = new Color(movingColor.r, movingColor.g, movingColor.b, _movingVal);

            var capturingColor = _config.CapturingRenderer.color;
            _config.CapturingRenderer.color = new Color(capturingColor.r, capturingColor.g, capturingColor.b, _capturingVal);
        }

        private void SoundTick()
        {
            if (_isMoving)
            {
                var tSource = SoundManager.Instance.PlaySoundAndGetSource(SoundType.SpacecraftMoving);
                if (tSource.IsCompleted)
                {
                    tSource.Result.pitch = _movingVal;
                }
            }
            else
            {
                SoundManager.Instance.StopSound(SoundType.SpacecraftMoving);
            }
        }
    }
}