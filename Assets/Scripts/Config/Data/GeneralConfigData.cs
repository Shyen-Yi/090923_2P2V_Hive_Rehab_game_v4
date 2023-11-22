using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public class GeneralConfig : GameConfigBase
	{
		private static GeneralConfigData _data;

		public GeneralConfig(GeneralSO so)
		{
			_data = new GeneralConfigData(so);
		}

		public static Int32 DefaultLevel => _data.DefaultLevel;
		public static Int32 MinLevel => _data.MinLevel;
		public static Int32 MaxLevel => _data.MaxLevel;
		public static Int32 DefaultGoal => _data.DefaultGoal;
		public static Int32 MinGoal => _data.MinGoal;
		public static Int32 MaxGoal => _data.MaxGoal;
		public static String name => _data.name;
		public static HideFlags hideFlags => _data.hideFlags;

	}

    
	public class GeneralConfigData : GameConfigDataBase
	{
		private GeneralSO _so;

		public Int32 DefaultLevel => _so.DefaultLevel;
		public Int32 MinLevel => _so.MinLevel;
		public Int32 MaxLevel => _so.MaxLevel;
		public Int32 DefaultGoal => _so.DefaultGoal;
		public Int32 MinGoal => _so.MinGoal;
		public Int32 MaxGoal => _so.MaxGoal;
		public String name => _so.name;
		public HideFlags hideFlags => _so.hideFlags;

		public GeneralConfigData(GeneralSO so)
		{
			_so = so;
		}
	}
}