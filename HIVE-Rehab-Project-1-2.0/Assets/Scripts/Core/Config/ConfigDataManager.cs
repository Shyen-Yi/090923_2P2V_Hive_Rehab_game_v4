using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class ConfigDataManager : SingletonBase<ConfigDataManager>, ICoreManager
    {
        private static readonly string SOResourcesPath = "Config";

        private Dictionary<Type, GameConfigBase> _dataDict;

        public void OnInit()
        {
            _dataDict = new Dictionary<Type, GameConfigBase>();

            // load all ConfigSO & init
            var allSO = Resources.LoadAll<GameSOBase>(SOResourcesPath);
            for (var i = 0; i < allSO.Length; ++i)
            {
                var so = allSO[i];
                var soType = so.GetType();
                var soTypeName = soType.GetFriendlyName();
                var index = soTypeName.IndexOf("SO");
                if (index >= 0)
                {
                    var key = soTypeName.Remove(index);
                    var config = Activator.CreateInstance(TypeUtil.GetType($"com.hive.projectr.{key}Config"), new object[] { so }) as GameConfigBase;
                    config.Init();

                    _dataDict[soType] = config;
                }
            }
        }

        public void OnDispose()
        {
            var types = new List<Type>(_dataDict.Keys);
            foreach (var type in types)
            {
                _dataDict[type].Dispose();
            }
            _dataDict.Clear();
            _dataDict = null;

            Resources.UnloadUnusedAssets();
        }
    }
}