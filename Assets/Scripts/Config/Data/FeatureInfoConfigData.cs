using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class FeatureInfoConfig : GameConfigBase
	{
		private static FeatureInfoSO _so;
		private static Dictionary<int, FeatureInfoConfigData> _dict;

		public FeatureInfoConfig(FeatureInfoSO so)
		{
			_so = so;
		}

		public static FeatureInfoConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, FeatureInfoConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new FeatureInfoConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in FeatureInfoSO!");
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

    
	public partial class FeatureInfoConfigData : GameConfigDataBase
	{
		private FeatureInfoSOItem _item;

		public FeatureType Feature => _item.Feature;
		public String Title => _item.Title;
		public String Desc => _item.Desc;

		public FeatureInfoConfigData(FeatureInfoSOItem item)
		{
			_item = item;
		}
	}
}