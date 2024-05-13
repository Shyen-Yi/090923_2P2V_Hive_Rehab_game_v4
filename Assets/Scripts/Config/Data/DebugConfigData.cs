using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    
	public partial class DebugConfig : GameConfigBase
	{
		private static DebugSO _so;
		private static Dictionary<int, DebugConfigData> _dict;

		public DebugConfig(DebugSO so)
		{
			_so = so;
		}

		public static DebugConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, DebugConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new DebugConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in DebugSO!");
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

    
	public partial class DebugConfigData : GameConfigDataBase
	{
		private DebugSOItem _item;

		public Boolean EnableCalibration => _item.EnableCalibration;
		public Boolean EnableKeyboardCursorControl => _item.EnableKeyboardCursorControl;
		public Boolean ShowBuiltInCursor => _item.ShowBuiltInCursor;

		public DebugConfigData(DebugSOItem item)
		{
			_item = item;
		}
	}
}