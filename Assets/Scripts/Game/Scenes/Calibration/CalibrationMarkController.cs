using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class CalibrationMarkController
    {
        private GeneralWidgetConfig _config;

        private static readonly int StartTriggerHash = Animator.StringToHash("Start");
        private static readonly int ResetTriggerHash = Animator.StringToHash("Reset");

        #region Extra
        private enum ExtraAnimator
        {
            Main = 0,
        }

        private Animator _animator;
        #endregion

        public CalibrationMarkController(GeneralWidgetConfig config)
        {
            _config = config;
            InitExtra();
        }

        private void InitExtra()
        {
            _animator = _config.ExtraAnimators[(int)ExtraAnimator.Main];
        }

        public void Activate()
        {
            // set active
            _config.gameObject.SetActive(true);

            // play animation
            _animator.SetTrigger(StartTriggerHash);
        }

        public void Deactivate()
        {
            // set inactive
            _config.gameObject.SetActive(false);
            
            // reset animation
            _animator.SetTrigger(ResetTriggerHash);
        }

        public void MoveToWorldPos(Vector3 worldPos)
        {
            _config.transform.position = new Vector3(worldPos.x, worldPos.y, _config.transform.position.z);
        }

        public Vector3 GetCurrentWorldPos()
        {
            return _config.transform.position;
        }
    }
}