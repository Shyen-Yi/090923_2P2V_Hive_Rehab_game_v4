using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class FeatureInfoConfig
	{
		private static Dictionary<FeatureType, List<FeatureInfoConfigData>> _typeDict;

		private void PostInit()
		{
			_typeDict = new Dictionary<FeatureType, List<FeatureInfoConfigData>>();

			foreach (var data in _dict.Values)
            {
				if (!_typeDict.TryGetValue(data.Feature, out var list))
				{
					list = new List<FeatureInfoConfigData>();
					_typeDict[data.Feature] = list;
				}
				list.Add(data);
			}
		}

		private void PostDispose()
		{
			_typeDict.Clear();
			_typeDict = null;
		}

		public static List<FeatureInfoConfigData> GetDataForFeature(FeatureType type)
        {
			if (_typeDict.TryGetValue(type, out var list))
            {
				return list;
            }

			return new List<FeatureInfoConfigData>();
        }
	}

    
	public partial class FeatureInfoConfigData
	{
		
	}

}