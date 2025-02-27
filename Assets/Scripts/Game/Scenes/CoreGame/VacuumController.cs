using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class VacuumController
    {
        public bool IsActive => _isActive;

        private VacuumConfig _config;
        private bool _isActive;
        private Vector3 _initScale;
        private float _scaleFactor;

        private static readonly int AirIsActiveBoolHash = Animator.StringToHash("isActive");
        private static readonly int BaseIsActiveBoolHash = Animator.StringToHash("isActive");

        public VacuumController(VacuumConfig config)
        {
            _config = config;

            _initScale = _config.VisualScalableRoot.localScale;
        }

        public void Activate()
        {
            _isActive = true;

            _config.BaseConfig.Animator.SetBool(BaseIsActiveBoolHash, true);
            _config.AirConfig.Animator.SetBool(AirIsActiveBoolHash, true);

            SoundManager.Instance.PlaySound(SoundType.VacuumEnable);
            SoundManager.Instance.StopSound(SoundType.VacuumDisable);
        }

        public void Deactivate()
        {
            _isActive = false;

            _config.BaseConfig.Animator.SetBool(BaseIsActiveBoolHash, false);
            _config.AirConfig.Animator.SetBool(AirIsActiveBoolHash, false);

            SoundManager.Instance.PlaySound(SoundType.VacuumDisable);
            SoundManager.Instance.StopSound(SoundType.VacuumEnable);
        }

        public void InitScale(float scale)
        {
            _scaleFactor = scale;
            _config.VisualScalableRoot.localScale = new Vector3(_initScale.x * _scaleFactor, _initScale.y, _initScale.z);
        }

        public void InitPos()
        {
            if (CoreGameConfig.GetData().RandomizeVacuumPositionAlongScreen)
            {
                var size = Mathf.Max(_config.BaseConfig.ActiveRenderer.size.x, _config.BaseConfig.InactiveRenderer.size.x);
                var offset = size * (1 - _scaleFactor) / 2;
                var pos = Random.Range(-offset, offset);
                var oriVisualPos = _config.VisualScalableRoot.localPosition;
                _config.VisualScalableRoot.localPosition = new Vector3(pos, oriVisualPos.y, oriVisualPos.z);
            }
            else
            {
                _config.VisualScalableRoot.localPosition = Vector3.zero;
            }
        }
    }
}