using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "ScriptableObject/Config Files/InputConfig")]
    public class InputSO : GameSOBase
    {
        [SerializeField] private List<InputSOItem> _items;

        public List<InputSOItem> Items => _items;
    }

    [System.Serializable]
    public class InputSOItem
    {
        
		[SerializeField] private Single cursorPreHoldingCooldownTime;
		[SerializeField] private Single cursorClickHoldingTimeMax;
		[SerializeField] private Single cursorHoldingMaxScreenOffset;

		public Single CursorPreHoldingCooldownTime => cursorPreHoldingCooldownTime;
		public Single CursorClickHoldingTimeMax => cursorClickHoldingTimeMax;
		public Single CursorHoldingMaxScreenOffset => cursorHoldingMaxScreenOffset;
    }
}