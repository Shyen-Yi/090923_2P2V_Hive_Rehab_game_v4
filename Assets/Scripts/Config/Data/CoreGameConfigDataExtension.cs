using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class CoreGameConfig
	{
		private void PostInit()
		{
			
		}

		private void PostDispose()
		{
			
		}

		public static CoreGameConfigData GetData()
        {
			return _dict[1];
        }
	}

    
	public partial class CoreGameConfigData
	{
		
	}

}