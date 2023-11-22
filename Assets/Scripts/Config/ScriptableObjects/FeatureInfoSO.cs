using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    [System.Serializable]
    public class FeatureInfoSerializableDictionary : SerializableDictionary<FeatureType, FeatureInfo>
    { }

    [System.Serializable]
    public class FeatureInfo
    {
        public string desc;
    }

    [CreateAssetMenu(fileName = "FeatureInfoConfig", menuName = "ScriptableObject/Config Files/FeatureInfoConfig")]
    public class FeatureInfoSO : GameSOBase
    {
        [SerializeField] FeatureInfoSerializableDictionary _featureInfoDict;

        public FeatureInfoSerializableDictionary FeatureInfoDict => _featureInfoDict;
    }
}