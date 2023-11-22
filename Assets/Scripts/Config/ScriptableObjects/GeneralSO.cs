using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "GeneralConfig", menuName = "ScriptableObject/Config Files/GeneralConfig")]
    public class GeneralSO : GameSOBase
    {
        [SerializeField] private int _defaultLevel;
        [SerializeField] private int _minLevel;
        [SerializeField] private int _maxLevel;
        [SerializeField] private int _defaultGoal;
        [SerializeField] private int _minGoal;
        [SerializeField] private int _maxGoal;

        public int DefaultLevel => _defaultLevel;
        public int MinLevel => _minLevel;
        public int MaxLevel => _maxLevel;
        public int DefaultGoal => _defaultGoal;
        public int MinGoal => _minGoal;
        public int MaxGoal => _maxGoal;
    }
}