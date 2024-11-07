using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    
	public partial class InputConfig : GameConfigBase
	{
		private static InputSO _so;
		private static Dictionary<int, InputConfigData> _dict;

		public InputConfig(InputSO so)
		{
			_so = so;
		}

		public static InputConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, InputConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new InputConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in InputSO!");
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

    
	public partial class InputConfigData : GameConfigDataBase
	{
		private InputSOItem _item;

		public Single CursorPreHoldingCooldownTime => _item.CursorPreHoldingCooldownTime;
		public Single CursorClickHoldingTimeMax => _item.CursorClickHoldingTimeMax;
		public Single CursorHoldingMaxScreenOffset => _item.CursorHoldingMaxScreenOffset;

		public InputConfigData(InputSOItem item)
		{
			_item = item;
		}
	}
}