using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace com.hive.projectr
{
    public class VacuumController
    {
        public bool IsActive => _isActive;

        private VacuumConfig _config;
        private bool _isActive;
        private Vector3 _initScale;

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

        public void InitVisual(float scale)
        {
            // scale
            _config.VisualScalableRoot.localScale = new Vector3(_initScale.x * scale, _initScale.y, _initScale.z);

            // pos
            if (CoreGameConfig.GetData().RandomizeVacuumPositionAlongScreen)
            {
                var size = Mathf.Max(_config.BaseConfig.ActiveRenderer.size.x, _config.BaseConfig.InactiveRenderer.size.x);
                var offset = size * (1 - scale) / 2;
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