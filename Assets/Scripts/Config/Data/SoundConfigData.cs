using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    
	public partial class SoundConfig : GameConfigBase
	{
		private static SoundSO _so;
		private static Dictionary<int, SoundConfigData> _dict;

		public SoundConfig(SoundSO so)
		{
			_so = so;
		}

		public static SoundConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, SoundConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new SoundConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in SoundSO!");
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

    
	public partial class SoundConfigData : GameConfigDataBase
	{
		private SoundSOItem _item;

		public SoundType Type => _item.Type;
		public AssetReference Clip => _item.Clip;
		public SoundPlayType PlayType => _item.PlayType;
		public Boolean IsLoop => _item.IsLoop;
		public Boolean NeedCache => _item.NeedCache;

		public SoundConfigData(SoundSOItem item)
		{
			_item = item;
		}
	}
}