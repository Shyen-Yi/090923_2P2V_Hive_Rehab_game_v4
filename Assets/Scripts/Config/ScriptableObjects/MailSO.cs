using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "MailConfig", menuName = "ScriptableObject/Config Files/MailConfig")]
    public class MailSO : GameSOBase
    {
        [SerializeField] private List<MailSOItem> _items;

        public List<MailSOItem> Items => _items;
    }

    [System.Serializable]
    public class MailSOItem
    {
        
		[SerializeField] private String dataSenderAccount;
		[SerializeField] private String dataSenderPassword;
		[SerializeField] private String dataReceiverAccount;

		public String DataSenderAccount => dataSenderAccount;
		public String DataSenderPassword => dataSenderPassword;
		public String DataReceiverAccount => dataReceiverAccount;
    }
}