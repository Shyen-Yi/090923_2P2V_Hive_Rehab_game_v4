using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "SampleConfig", menuName = "ScriptableObject/Config Files/SampleConfig")]
    public class SampleSO : GameSOBase
    {
        [SerializeField] private int _testInt;
        [SerializeField] private string _testString;

        public int TestInt => _testInt;
        public string TestString => _testString;
    }
}