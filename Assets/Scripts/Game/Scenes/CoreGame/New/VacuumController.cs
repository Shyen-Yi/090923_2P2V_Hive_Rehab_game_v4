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

            _initScale = _config.transform.localScale;
        }

        public void Activate()
        {
            _isActive = true;

            _config.BaseConfig.Animator.SetBool(BaseIsActiveBoolHash, true);
            _config.AirConfig.Animator.SetBool(AirIsActiveBoolHash, true);
        }

        public void Deactivate()
        {
            _isActive = false;

            _config.BaseConfig.Animator.SetBool(BaseIsActiveBoolHash, false);
            _config.AirConfig.Animator.SetBool(AirIsActiveBoolHash, false);
        }

        public void SetAirSize(float scale)
        {
            _config.transform.localScale = new Vector3(_initScale.x * scale, _initScale.y, _initScale.z);
        }
    }
}