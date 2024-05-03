using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class InputConfig
	{
		private void PostInit()
		{
			
		}

		private void PostDispose()
		{
			
		}

		public static InputConfigData GetData()
        {
			return _dict[1];
        }
	}

    
	public partial class InputConfigData
	{
		
	}

}