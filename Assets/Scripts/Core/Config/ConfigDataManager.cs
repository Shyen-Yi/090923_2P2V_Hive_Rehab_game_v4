using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class ConfigDataManager
    /// @brief Manages configuration data by loading and initializing `ConfigSO` assets and their corresponding `ConfigData` objects.
    /// 
    /// The `ConfigDataManager` class is responsible for loading all configuration ScriptableObjects (ConfigSO) from the specified
    /// resources path, initializing their corresponding `ConfigData` objects, and storing them in a dictionary. It also handles
    /// disposing of the loaded data when the application is closed or when it is no longer needed.
    public class ConfigDataManager : SingletonBase<ConfigDataManager>, ICoreManager
    {
        private static readonly string SOResourcesPath = "Config";

        private Dictionary<Type, GameConfigBase> _dataDict;

        /// <summary>
        /// Initializes the ConfigDataManager by loading all configuration ScriptableObjects and their corresponding ConfigData objects.
        /// </summary>
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

        /// <summary>
        /// Disposes of all loaded configuration data and clears the dictionary.
        /// </summary>
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