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
        }

        public void Deactivate()
        {
            _isActive = false;

            _config.BaseConfig.Animator.SetBool(BaseIsActiveBoolHash, false);
            _config.AirConfig.Animator.SetBool(AirIsActiveBoolHash, false);

            SoundManager.Instance.PlaySound(SoundType.VacuumDisable);
        }

        public void SetAirSize(float scale)
        {
            _config.VisualScalableRoot.localScale = new Vector3(_initScale.x * scale, _initScale.y, _initScale.z);
        }
    }
}