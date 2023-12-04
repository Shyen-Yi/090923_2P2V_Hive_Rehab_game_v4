using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    [System.Serializable]
    public class AsteroidConfig : GeneralWidgetConfig
    {
        public Action<Collision2D> onCollisionEnter2D;
        public Action<Collider2D> onTriggerEnter2D;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            onCollisionEnter2D?.Invoke(collision);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onTriggerEnter2D?.Invoke(collision);
        }
    }
}