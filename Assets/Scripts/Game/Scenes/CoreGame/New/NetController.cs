using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class NetController
    {
        #region Extra
        private enum ExtraAnimator
        {
            Main = 0,
        }

        private Animator _animator;
        #endregion

        private GeneralWidgetConfig _config;

        private static readonly int SpawnTriggerHash = Animator.StringToHash("Spawn");

        public NetController(GeneralWidgetConfig config)
        {
            _config = config;

            InitExtra();
        }

        private void InitExtra()
        {
            _animator = _config.ExtraAnimators[(int)ExtraAnimator.Main];
        }

        public void PlaySpawnAnimation()
        {
            _animator.SetTrigger(SpawnTriggerHash);
        }
    }
}