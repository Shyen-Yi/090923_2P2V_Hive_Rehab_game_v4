using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    
	public partial class MailConfig
	{
		private void PostInit()
		{
			
		}

		private void PostDispose()
		{
			
		}

		public static MailConfigData GetData()
		{
			return _dict[1];
		}
    }

    
	public partial class MailConfigData
	{
		
	}

}