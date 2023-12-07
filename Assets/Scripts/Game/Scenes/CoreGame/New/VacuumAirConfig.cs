using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class VacuumAirConfig : GeneralWidgetConfig
    {
        [SerializeField] private VacuumConfig _vacuumRootConfig;
        [SerializeField] private Animator _animator;

        public VacuumConfig VacuumRootConfig => _vacuumRootConfig;
        public Animator Animator => _animator;
    }
}