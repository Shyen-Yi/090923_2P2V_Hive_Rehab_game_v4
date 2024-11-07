using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class CoreGameLevelConfig
	{
		public static int MinLevel = int.MaxValue;
		public static int MaxLevel = int.MinValue;

		private static Dictionary<int, CoreGameLevelConfigData> _levelDict;

		private void PostInit()
		{
			_levelDict = new Dictionary<int, CoreGameLevelConfigData>();

			foreach (var data in _dict.Values)
			{
				var level = data.Level;
				if (!_levelDict.ContainsKey(level))
				{
					_levelDict[level] = data;
				}

				MinLevel = Mathf.Min(MinLevel, level);
				MaxLevel = Mathf.Max(MaxLevel, level);
            }
		}

		private void PostDispose()
		{
			_levelDict.Clear();
			_levelDict = null;
        }

		public static CoreGameLevelConfigData GetLevelData(int level)
		{
			if (_levelDict.TryGetValue(level, out var data))
			{
				return data;
			}

			return null;
		}
	}

    
	public partial class CoreGameLevelConfigData
	{
		
	}

}