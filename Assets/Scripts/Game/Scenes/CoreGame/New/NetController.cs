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

        private enum ExtraInt
        {
            Opacity = 0,
        }

        private Animator _animator;

        private float _opacity;
        #endregion

        private GeneralWidgetConfig _config;

        private static readonly int SpawnTriggerHash = Animator.StringToHash("Spawn");

        public NetController(GeneralWidgetConfig config)
        {
            _config = config;

            InitExtra();

            foreach (var spriteRenderer in _config.GetComponentsInChildren<SpriteRenderer>())
            {
                var color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, _opacity);
            }
        }

        private void InitExtra()
        {
            _animator = _config.ExtraAnimators[(int)ExtraAnimator.Main];

            _opacity = Mathf.Clamp01(_config.ExtraInts[(int)ExtraInt.Opacity] / 1000f);
        }

        public void PlaySpawnAnimation()
        {
            _animator.SetTrigger(SpawnTriggerHash);
        }
    }
}