using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class PerformanceConfig
	{
		private static Dictionary<PerformanceType, PerformanceConfigData> _typeDict;

		private void PostInit()
		{
			_typeDict = new Dictionary<PerformanceType, PerformanceConfigData>();
			foreach (var pair in _dict)
            {
				var configData = pair.Value;
				if (!_typeDict.ContainsKey(configData.Type))
                {
					_typeDict[configData.Type] = configData;
                }
            }
		}

		private void PostDispose()
		{
			
		}

		public static PerformanceConfigData GetData(PerformanceType performance)
        {
			if (_typeDict.TryGetValue(performance, out var configData))
            {
				return configData;
            }

			return null;
        }
	}

    
	public partial class PerformanceConfigData
	{
		
	}

}