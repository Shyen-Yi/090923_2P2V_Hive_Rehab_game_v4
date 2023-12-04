using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class AsteroidController
    {
        private AsteroidConfig _config;

        public AsteroidController(AsteroidConfig config)
        {
            _config = config;
        }

        public void Init()
        {
            _config.onCollisionEnter2D += OnCollisionEnter2D;
            _config.onTriggerEnter2D += OnTriggerEnter2D;

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void Dispose()
        {
            _config.onCollisionEnter2D -= OnCollisionEnter2D;
            _config.onTriggerEnter2D -= OnTriggerEnter2D;

            MonoBehaviourUtil.OnUpdate -= Tick;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("VacuumBase"))
            {
                // reflect

            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("VacuumAir"))
            {
                // 
            }
        }

        private void Tick()
        {
            // spin
            var coreGameData = CoreGameConfig.GetData();
            if (coreGameData != null)
            {
                _config.transform.Rotate(Vector3.forward, coreGameData.AsteroidSpinRoundPerSec / 360f * Time.deltaTime);
            }
        }
    }
}