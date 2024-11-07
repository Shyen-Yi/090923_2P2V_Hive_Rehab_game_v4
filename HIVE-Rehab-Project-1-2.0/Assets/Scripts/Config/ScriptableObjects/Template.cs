using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "TemplateConfig", menuName = "ScriptableObject/Config Files/TemplateConfig")]
    public class TemplateSO : GameSOBase
    {
        [SerializeField] private List<TemplateSOItem> _items;

        public List<TemplateSOItem> Items => _items;
    }

    [System.Serializable]
    public class TemplateSOItem
    {
        //Headers Start
        //Headers End
    }
}