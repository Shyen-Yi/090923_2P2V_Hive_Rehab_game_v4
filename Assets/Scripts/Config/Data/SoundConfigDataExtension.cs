using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class SoundConfig
	{
		private static Dictionary<SoundType, SoundConfigData> _typeDict;

		private void PostInit()
		{
			_typeDict = new Dictionary<SoundType, SoundConfigData>();
			foreach (var configData in _dict.Values)
			{
				if (!_typeDict.ContainsKey(configData.Type))
				{
					_typeDict[configData.Type] = configData;
				}
			}
        }

		private void PostDispose()
		{
			_typeDict.Clear();
			_typeDict = null;
        }

		public static SoundConfigData GetData(SoundType type)
		{
			if (_typeDict.TryGetValue(type, out var data))
			{
				return data;
			}

			return null;
		}
	}

    
	public partial class SoundConfigData
	{
		
	}

}