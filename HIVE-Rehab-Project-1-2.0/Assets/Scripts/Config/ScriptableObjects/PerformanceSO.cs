using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "PerformanceConfig", menuName = "ScriptableObject/Config Files/PerformanceConfig")]
    public class PerformanceSO : GameSOBase
    {
        [SerializeField] private List<PerformanceSOItem> _items;

        public List<PerformanceSOItem> Items => _items;
    }

    [System.Serializable]
    public class PerformanceSOItem
    {
        
		[SerializeField] private PerformanceType type;
		[SerializeField] private String desc;

		public PerformanceType Type => type;
		public String Desc => desc;
    }
}