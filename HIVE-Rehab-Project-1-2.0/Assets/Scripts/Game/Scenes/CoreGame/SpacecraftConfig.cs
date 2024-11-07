using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class SpacecraftConfig : GeneralWidgetConfig
    {
        [SerializeField] private SpriteRenderer _fireLeftRenderer;
        [SerializeField] private SpriteRenderer _fireRightRenderer;
        [SerializeField] private SpriteRenderer _idleRenderer;
        [SerializeField] private SpriteRenderer _movingRenderer;
        [SerializeField] private SpriteRenderer _capturingRenderer;
        [SerializeField] private CircleCollider2D _circleCollider;

        public SpriteRenderer FireLeftRenderer => _fireLeftRenderer;
        public SpriteRenderer FireRightRenderer => _fireRightRenderer;
        public SpriteRenderer IdleRenderer => _idleRenderer;
        public SpriteRenderer MovingRenderer => _movingRenderer;
        public SpriteRenderer CapturingRenderer => _capturingRenderer;
        public CircleCollider2D CircleCollider => _circleCollider;
    }
}