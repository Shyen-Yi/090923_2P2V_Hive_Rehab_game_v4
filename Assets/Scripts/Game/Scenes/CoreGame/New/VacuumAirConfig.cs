using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class VacuumAirConfig : GeneralWidgetConfig
    {
        [SerializeField] private Animator _animator;

        public Animator Animator => _animator;
    }
}