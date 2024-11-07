using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    
	public partial class CoreGameConfig : GameConfigBase
	{
		private static CoreGameSO _so;
		private static Dictionary<int, CoreGameConfigData> _dict;

		public CoreGameConfig(CoreGameSO so)
		{
			_so = so;
		}

		public static CoreGameConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, CoreGameConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new CoreGameConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in CoreGameSO!");
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

    
	public partial class CoreGameConfigData : GameConfigDataBase
	{
		private CoreGameSOItem _item;

		public Single AsteroidSpinRoundPerSec => _item.AsteroidSpinRoundPerSec;
		public Int32 StartCountdownSec => _item.StartCountdownSec;
		public Int32 InfoTextUpdateSec => _item.InfoTextUpdateSec;
		public Single GameProgressFillSec => _item.GameProgressFillSec;
		public Single AsteroidWarningSec => _item.AsteroidWarningSec;
		public Single ArrowDistanceFromScreenEdge => _item.ArrowDistanceFromScreenEdge;

		public CoreGameConfigData(CoreGameSOItem item)
		{
			_item = item;
		}
	}
}