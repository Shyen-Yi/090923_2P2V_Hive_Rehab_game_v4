using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class CalibrationStageConfig : GameConfigBase
	{
		private static CalibrationStageSO _so;
		private static Dictionary<int, CalibrationStageConfigData> _dict;

		public CalibrationStageConfig(CalibrationStageSO so)
		{
			_so = so;
		}

		public static CalibrationStageConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, CalibrationStageConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new CalibrationStageConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in CalibrationStageSO!");
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

    
	public partial class CalibrationStageConfigData : GameConfigDataBase
	{
		private CalibrationStageSOItem _item;

		public CalibrationStageType Stage => _item.Stage;
		public Int32 SortingId => _item.SortingId;
		public String Instruction => _item.Instruction;
		public String InstructionWhenHolding => _item.InstructionWhenHolding;
		public Int32 MaxHoldingCheckCount => _item.MaxHoldingCheckCount;
		public Single EachHoldingCheckDuration => _item.EachHoldingCheckDuration;
		public Single HoldingPreparationTime => _item.HoldingPreparationTime;
		public Single CooldownTime => _item.CooldownTime;

		public CalibrationStageConfigData(CalibrationStageSOItem item)
		{
			_item = item;
		}
	}
}