using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    
	public partial class PerformanceConfig : GameConfigBase
	{
		private static PerformanceSO _so;
		private static Dictionary<int, PerformanceConfigData> _dict;

		public PerformanceConfig(PerformanceSO so)
		{
			_so = so;
		}

		public static PerformanceConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, PerformanceConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new PerformanceConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in PerformanceSO!");
				}
			}

			PostInit();
		}

		protected override void OnDispose()
		{
			_dict.Clear();
			_dict = null;
			_so = null;

			PostDispose();
		}
	}

    
	public partial class PerformanceConfigData : GameConfigDataBase
	{
		private PerformanceSOItem _item;

		public PerformanceType Type => _item.Type;
		public String Desc => _item.Desc;

		public PerformanceConfigData(PerformanceSOItem item)
		{
			_item = item;
		}
	}
}