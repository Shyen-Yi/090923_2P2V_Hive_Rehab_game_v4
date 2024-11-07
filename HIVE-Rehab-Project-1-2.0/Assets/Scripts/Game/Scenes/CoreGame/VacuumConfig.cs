using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public enum VacuumType
    {
        Top,
        Bottom,
        Left,
        Right,
    }

    public class VacuumConfig : GeneralWidgetConfig
    {
        [SerializeField] private VacuumType _type;
        [SerializeField] private VacuumBaseConfig _baseConfig;
        [SerializeField] private VacuumAirConfig _airConfig;
        [SerializeField] private Transform _visualScalableRoot;

        public VacuumType Type => _type;
        public VacuumBaseConfig BaseConfig => _baseConfig;
        public VacuumAirConfig AirConfig => _airConfig;
        public Transform VisualScalableRoot => _visualScalableRoot;
    }
}