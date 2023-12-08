using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting.FullSerializer;

namespace com.hive.projectr
{
    public struct AsteroidData
    {
        public Vector3 startWorldPos;
        public Vector3 startDirection;
        public float speed;
        public float size;
        public float lifeTime;
        public AnimationCurve movementCurve;
        public Action<int> onEnterVacuumAir;
        public Action<int> onLifetimeRunOut;
        public Action<int> onCaptured;

        public AsteroidData(Vector3 startWorldPos, Vector3 startDirection, float speed, float size, float lifeTime, AnimationCurve movementCurve, Action<int> onEnterVacuumAir, Action<int> onLifetimeRunOut, Action<int> onCaptured)
        {
            this.startWorldPos = startWorldPos;
            this.startDirection = startDirection;
            this.speed = speed;
            this.size = size;
            this.lifeTime = lifeTime;
            this.movementCurve = movementCurve;
            this.onEnterVacuumAir = onEnterVacuumAir;
            this.onLifetimeRunOut = onLifetimeRunOut;
            this.onCaptured = onCaptured;
        }
    }

    public class AsteroidController
    {
        public int Id { get; private set; }

        private AsteroidConfig _config;
        private bool _isRunning;
        private Transform _capturingOwner;
        private Vector3 _offsetFromOwner;
        private Action<int> _onEnterVacuumAir;
        private Action<int> _onLifetimeRunOut;
        private Action<int> _onCaptured;
        private float _remainingLifetime;
        private float _totalLifetime;

        private static int UId = 0;

        public AsteroidController(AsteroidConfig config)
        {
            _config = config;
            Id = ++UId;
        }

        public void Start(AsteroidData data)
        {
            if (_isRunning)
            {
                Logger.LogError($"Asteroid is already running! Id: {Id}");
                return;
            }

            _onEnterVacuumAir = data.onEnterVacuumAir;
            _onLifetimeRunOut = data.onLifetimeRunOut;
            _onCaptured = data.onCaptured;

            _totalLifetime = _remainingLifetime = data.lifeTime;

            _config.transform.position = data.startWorldPos;
            _config.transform.localScale = Vector3.one * data.size;
            _config.Rigidbody2D.velocity = data.startDirection.normalized * data.speed;

            _config.RotationRoot.localScale = Vector3.one;
            _config.Renderer.color = Color.white;

            _isRunning = true;
        }

        public void Stop(Action onFinished)
        {
            if (!_isRunning)
            {
                Logger.LogError($"Asteroid is not running! Id: {Id}");
                return;
            }

            _isRunning = false;

            Reset();

            DOTween.To(() => _config.RotationRoot.localScale, x => _config.RotationRoot.localScale = x, Vector3.one * _config.StopScale, _config.StopDuration)
                .OnStart(() =>
                {
                    var color = _config.Renderer.color;
                    DOTween.To(() => _config.Renderer.color, x => _config.Renderer.color = x, new Color(color.r, color.g, color.b, 0), _config.StopDuration);
                })
                .OnComplete(() =>
                {
                    onFinished?.Invoke();
                });
        }

        public void Activate()
        {
            _config.onCollisionEnter2D += OnCollisionEnter2D;
            _config.onTriggerEnter2D += OnTriggerEnter2D;

            MonoBehaviourUtil.OnUpdate += Tick;

            _config.gameObject.SetActive(true);

            _config.RotationRoot.rotation = Quaternion.identity;
            Reset();
        }

        private void Reset()
        {
            _config.Rigidbody2D.velocity = Vector2.zero;
            _capturingOwner = null;
        }

        public void Deactivate()
        {
            _config.onCollisionEnter2D -= OnCollisionEnter2D;
            _config.onTriggerEnter2D -= OnTriggerEnter2D;

            MonoBehaviourUtil.OnUpdate -= Tick;

            _config.gameObject.SetActive(false);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_isRunning)
                return;
        }

        public float GetLifetimeProgress()
        {
            return Mathf.Clamp01((_totalLifetime - _remainingLifetime) / _totalLifetime);
        }

        private bool IsCaptured()
        {
            return _capturingOwner != null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isRunning)
                return;

            if (collision.gameObject.CompareTag(TagNames.VacuumAir) && IsCaptured())
            {
                _onEnterVacuumAir?.Invoke(Id);
            }
            else if (collision.gameObject.CompareTag(TagNames.Spacecraft))
            {
                _capturingOwner = collision.transform;
                _offsetFromOwner = _config.transform.position - _capturingOwner.position;
                _config.Rigidbody2D.velocity = Vector2.zero;
                _onCaptured?.Invoke(Id);
            }
            else if (collision.gameObject.CompareTag(TagNames.VacuumBase))
            {
                // reflect
                if (collision.gameObject.TryGetComponent<VacuumBaseConfig>(out var baseConfig))
                {
                    var velocity = _config.Rigidbody2D.velocity;
                    switch (baseConfig.VacuumRootConfig.Type)
                    {
                        case VacuumType.Top:
                        case VacuumType.Bottom:
                            velocity = new Vector2(velocity.x, -velocity.y);
                            break;
                        case VacuumType.Left:
                        case VacuumType.Right:
                            velocity = new Vector2(-velocity.x, velocity.y);
                            break;
                        default:
                            Logger.LogError($"Undefined type: {baseConfig.VacuumRootConfig.Type}");
                            break;
                    }

                    var energyLossFactor = 1f;
                    velocity *= energyLossFactor;
                    _config.Rigidbody2D.velocity = velocity;
                }
            }
        }

        private void Tick()
        {
            if (!_isRunning)
                return;

            // check lifetime first
            _remainingLifetime -= Time.deltaTime;
            var lifetimeProgress = GetLifetimeProgress();
            _config.Renderer.color = new Color(1, 1 - lifetimeProgress, 1 - lifetimeProgress, 1);

            if (_remainingLifetime < 0)
            {
                _onLifetimeRunOut?.Invoke(Id);
                return;
            }

            // held by/following owner
            if (IsCaptured())
            {
                _config.transform.position = _capturingOwner.position + _offsetFromOwner;
                return;
            }

            // spin
            var coreGameData = CoreGameConfig.GetData();
            if (coreGameData != null)
            {
                var degree = (coreGameData.AsteroidSpinRoundPerSec * 360f * Time.deltaTime) % 360;
                _config.RotationRoot.Rotate(Vector3.forward, degree);
            }
        }
    }
}