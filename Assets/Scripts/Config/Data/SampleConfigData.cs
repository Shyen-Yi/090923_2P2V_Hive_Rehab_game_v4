using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public class SampleConfig : GameConfigBase
	{
		private static SampleConfigData _data;

		public SampleConfig(SampleSO so)
		{
			_data = new SampleConfigData(so);
		}

		public static Int32 TestInt => _data.TestInt;
		public static String TestString => _data.TestString;
		public static String name => _data.name;
		public static HideFlags hideFlags => _data.hideFlags;

	}

    
	public class SampleConfigData : GameConfigDataBase
	{
		private SampleSO _so;

		public Int32 TestInt => _so.TestInt;
		public String TestString => _so.TestString;
		public String name => _so.name;
		public HideFlags hideFlags => _so.hideFlags;

		public SampleConfigData(SampleSO so)
		{
			_so = so;
		}
	}
}