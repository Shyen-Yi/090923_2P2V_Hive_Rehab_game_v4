using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "DebugConfig", menuName = "ScriptableObject/Config Files/DebugConfig")]
    public class DebugSO : GameSOBase
    {
        [SerializeField] private List<DebugSOItem> _items;

        public List<DebugSOItem> Items => _items;
    }

    [System.Serializable]
    public class DebugSOItem
    {
        
		[SerializeField] private Boolean enableCalibration;
		[SerializeField] private Boolean enableKeyboardCursorControl;

		public Boolean EnableCalibration => enableCalibration;
		public Boolean EnableKeyboardCursorControl => enableKeyboardCursorControl;
    }
}