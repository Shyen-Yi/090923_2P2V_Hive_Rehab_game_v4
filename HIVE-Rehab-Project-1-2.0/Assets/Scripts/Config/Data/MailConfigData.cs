using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    
	public partial class MailConfig : GameConfigBase
	{
		private static MailSO _so;
		private static Dictionary<int, MailConfigData> _dict;

		public MailConfig(MailSO so)
		{
			_so = so;
		}

		public static MailConfigData GetData(int id)
		{
			if (_dict.TryGetValue(id, out var data))
			{
				return data;
			}
			return null;
		}

		protected override void OnInit()
		{
			_dict = new Dictionary<int, MailConfigData>();

			for (var i = 0; i < _so.Items.Count; ++i)
			{
				var id = i + 1;
				if (!_dict.ContainsKey(id))
				{
					_dict[id] = new MailConfigData(_so.Items[i]);
				}
				else
				{
					Logger.LogError($"Duplicate id: {id} in MailSO!");
				}
			}

			PostInit();
		}

		protected override void OnDispose()
		{
			_dict.Clear();
			_dict = null;
			_so = null;

			PostDispose();
		}
	}

    
	public partial class MailConfigData : GameConfigDataBase
	{
		private MailSOItem _item;

		public String DataSenderAccount => _item.DataSenderAccount;
		public String DataSenderPassword => _item.DataSenderPassword;
		public String DataReceiverAccount => _item.DataReceiverAccount;

		public MailConfigData(MailSOItem item)
		{
			_item = item;
		}
	}
}