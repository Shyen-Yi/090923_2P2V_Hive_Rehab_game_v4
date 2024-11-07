using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class CalibrationStageConfig
	{
		private void PostInit()
		{
		}

		private void PostDispose()
		{
		}

		private static int CmpData(CalibrationStageConfigData a, CalibrationStageConfigData b)
        {
			if (a == null || b == null)
				return 0;

			return a.SortingId - b.SortingId;
        }

		public static List<CalibrationStageConfigData> GetAllStageData()
        {
			var list = new List<CalibrationStageConfigData>(_dict.Values);
			list.Sort(CmpData);
			return list;
		}
	}

    
	public partial class CalibrationStageConfigData
	{
		
	}

}