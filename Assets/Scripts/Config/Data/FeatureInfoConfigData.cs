using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public class FeatureInfoConfig : GameConfigBase
	{
		private static FeatureInfoConfigData _data;

		public FeatureInfoConfig(FeatureInfoSO so)
		{
			_data = new FeatureInfoConfigData(so);
		}

		public static FeatureInfoSerializableDictionary FeatureInfoDict => _data.FeatureInfoDict;
		public static String name => _data.name;
		public static HideFlags hideFlags => _data.hideFlags;

	}

    
	public class FeatureInfoConfigData : GameConfigDataBase
	{
		private FeatureInfoSO _so;

		public FeatureInfoSerializableDictionary FeatureInfoDict => _so.FeatureInfoDict;
		public String name => _so.name;
		public HideFlags hideFlags => _so.hideFlags;

		public FeatureInfoConfigData(FeatureInfoSO so)
		{
			_so = so;
		}
	}
}