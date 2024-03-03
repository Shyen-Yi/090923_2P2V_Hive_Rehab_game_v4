using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class CalibrationConfig : GameConfigBase
	{
		private static CalibrationSO _so;
		private static Dictionary<int, CalibrationConfigData> _dict;

		public CalibrationConfig(CalibrationSO so)
		{
			_so = so;
		}

		public static CalibrationConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, CalibrationConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new CalibrationConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in CalibrationSO!");
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

    
	public partial class CalibrationConfigData : GameConfigDataBase
	{
		private CalibrationSOItem _item;

		public Single ArrowWorldDistanceFromCenter => _item.ArrowWorldDistanceFromCenter;
		public Single StageErrorProtectionTriggerTime => _item.StageErrorProtectionTriggerTime;
		public Single HoldingMaxScreenOffset => _item.HoldingMaxScreenOffset;
		public Single PendingMarkerAlpha => _item.PendingMarkerAlpha;

		public CalibrationConfigData(CalibrationSOItem item)
		{
			_item = item;
		}
	}
}