using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class VacuumBaseConfig : GeneralWidgetConfig
    {
        [SerializeField] private VacuumConfig _vacuumRootConfig;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _activeRenderer;
        [SerializeField] private SpriteRenderer _inactiveRenderer;

        public VacuumConfig VacuumRootConfig => _vacuumRootConfig;
        public Animator Animator => _animator;
        public SpriteRenderer ActiveRenderer => _activeRenderer;
        public SpriteRenderer InactiveRenderer => _inactiveRenderer;
    }
}