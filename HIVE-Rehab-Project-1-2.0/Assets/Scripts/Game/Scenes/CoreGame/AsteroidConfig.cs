using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    [System.Serializable]
    [RequireComponent(typeof(Rigidbody2D))]
    public class AsteroidConfig : GeneralWidgetConfig
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Transform _rotationRoot;
        [SerializeField] private float _stopScale;
        [SerializeField] private float _startScale;
        [SerializeField] private float _stopDuration;

        public SpriteRenderer Renderer => _renderer;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public Transform RotationRoot => _rotationRoot;
        public float StopScale => _stopScale;
        public float StartScale => _startScale;
        public float StopDuration => _stopDuration;

        public Action<Collision2D> onCollisionEnter2D;
        public Action<Collider2D> onTriggerEnter2D;

        private void Awake()
        {
            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }
        }

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