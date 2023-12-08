using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class CoreGameLevelConfig : GameConfigBase
	{
		private static CoreGameLevelSO _so;
		private static Dictionary<int, CoreGameLevelConfigData> _dict;

		public CoreGameLevelConfig(CoreGameLevelSO so)
		{
			_so = so;
		}

		public static CoreGameLevelConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, CoreGameLevelConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new CoreGameLevelConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in CoreGameLevelSO!");
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

    
	public partial class CoreGameLevelConfigData : GameConfigDataBase
	{
		private CoreGameLevelSOItem _item;

		public Int32 Level => _item.Level;
		public Single AsteroidSpeed => _item.AsteroidSpeed;
		public Single AsteroidSize => _item.AsteroidSize;
		public Single AsteroidLifeTime => _item.AsteroidLifeTime;
		public AnimationCurve AsteroidMovement => _item.AsteroidMovement;
		public Single AsteroidSpawnGapSec => _item.AsteroidSpawnGapSec;
		public Single VacuumSize => _item.VacuumSize;
		public Single SpacecraftSize => _item.SpacecraftSize;
		public Int32 MaxAsteroidCount => _item.MaxAsteroidCount;
		public Int32 NumOfAsteroidCollectedToPass => _item.NumOfAsteroidCollectedToPass;

		public CoreGameLevelConfigData(CoreGameLevelSOItem item)
		{
			_item = item;
		}
	}
}