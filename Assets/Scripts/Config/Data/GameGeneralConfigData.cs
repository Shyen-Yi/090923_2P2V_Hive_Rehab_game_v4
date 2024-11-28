using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    
	public partial class GameGeneralConfig : GameConfigBase
	{
		private static GameGeneralSO _so;
		private static Dictionary<int, GameGeneralConfigData> _dict;

		public GameGeneralConfig(GameGeneralSO so)
		{
			_so = so;
		}

		public static GameGeneralConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, GameGeneralConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new GameGeneralConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in GameGeneralSO!");
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

    
	public partial class GameGeneralConfigData : GameConfigDataBase
	{
		private GameGeneralSOItem _item;

		public Int32 DefaultBlock => _item.DefaultBlock;
		public Int32 DefaultGoal => _item.DefaultGoal;
		public Single CoreGameTransitionSec => _item.CoreGameTransitionSec;
		public Single CalibrationTransitionSec => _item.CalibrationTransitionSec;
		public String DefaultUserName => _item.DefaultUserName;
		public String AdminUsername => _item.AdminUsername;
		public String AdminPassword => _item.AdminPassword;
		public Int32 SoundVolumePercentageWhenInBackground => _item.SoundVolumePercentageWhenInBackground;
		public Int32 DailyMaxAttempt => _item.DailyMaxAttempt;

		public GameGeneralConfigData(GameGeneralSOItem item)
		{
			_item = item;
		}
	}
}