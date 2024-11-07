using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class GameGeneralConfig
	{
		private void PostInit()
		{
			
		}

		private void PostDispose()
		{
			
		}

		public static GameGeneralConfigData GetData()
        {
			return _dict[1];
        }
	}

    
	public partial class GameGeneralConfigData
	{
		
	}

}