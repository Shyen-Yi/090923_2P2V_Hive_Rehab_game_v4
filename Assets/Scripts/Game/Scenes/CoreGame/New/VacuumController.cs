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

        private static readonly int AirActivateTriggerHash = Animator.StringToHash("Activate");
        private static readonly int AirDeactivateTriggerHash = Animator.StringToHash("Deactivate");
        private static readonly int BaseActivateTriggerHash = Animator.StringToHash("Activate");
        private static readonly int BaseDeactivateTriggerHash = Animator.StringToHash("Deactivate");

        public VacuumController(VacuumConfig config)
        {
            _config = config;
        }

        public void Activate()
        {
            _isActive = true;

            _config.BaseConfig.Animator.SetTrigger(BaseActivateTriggerHash);
            _config.AirConfig.Animator.SetTrigger(AirActivateTriggerHash);
        }

        public void Deactivate()
        {
            _isActive = false;

            _config.BaseConfig.Animator.SetTrigger(BaseDeactivateTriggerHash);
            _config.AirConfig.Animator.SetTrigger(AirDeactivateTriggerHash);
        }
    }
}