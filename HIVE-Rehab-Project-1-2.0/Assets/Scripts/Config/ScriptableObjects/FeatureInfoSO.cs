using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "FeatureInfoConfig", menuName = "ScriptableObject/Config Files/FeatureInfoConfig")]
    public class FeatureInfoSO : GameSOBase
    {
        [SerializeField] private List<FeatureInfoSOItem> _items;

        public List<FeatureInfoSOItem> Items => _items;
    }

    [System.Serializable]
    public class FeatureInfoSOItem
    {
        
		[SerializeField] private FeatureType feature;
		[SerializeField] private String title;
		[SerializeField] private String desc;

        public FeatureType Feature => feature;
		public String Title => title;
		public String Desc => desc;
    }
}