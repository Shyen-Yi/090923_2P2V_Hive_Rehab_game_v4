using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class CalibrationConfig
	{
		private void PostInit()
		{
			
		}

		private void PostDispose()
		{
			
		}

		public static CalibrationConfigData GetData()
        {
			return _dict[1];
        }
	}

    
	public partial class CalibrationConfigData
	{
		
	}

}