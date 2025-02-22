using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace com.hive.projectr
{
    /// @ingroup Editor
    /// @class ConfigDataGenerator
    /// @brief A Unity editor tool for generating `ConfigData` and `ConfigDataExtension` script files based on `ConfigSO` ScriptableObject files.
    /// 
    /// The `ConfigDataGenerator` class is a custom Unity editor tool that automates the generation of `ConfigData` and `ConfigDataExtension`
    /// script files based on the `ConfigSO` ScriptableObject files located in a specified directory. It processes these assets and creates
    /// corresponding configuration files for each `ConfigSO` class in the project. The tool provides a user interface within the Unity Editor
    /// for setting paths, triggering the generation, and managing the configuration data.
    public class ConfigDataGenerator : EditorWindow
    {
        // Template paths and configuration paths
        private static readonly string ConfigDataTemplatePath = "Assets/Scripts/Core/Config/ConfigDataTemplate.cs";
        private static readonly string ConfigDataExtensionTemplatePath = "Assets/Scripts/Core/Config/ConfigDataExtensionTemplate.cs";
        private static readonly string ConfigSOPath = "Assets/Resources/Config";
        private static readonly string ConfigDataPath = "Assets/Scripts/Config/Data";

        private static string _configSOPath = ConfigSOPath;
        private static string _configDataPath = ConfigDataPath;

        /// <summary>
        /// Creates and shows the ConfigDataGenerator window in the Unity Editor.
        /// </summary>
        [MenuItem("Tools/Hive Toolkit/Config Data Generator"),]
        public static void ShowWindow()
        {
            ConfigDataGenerator window = GetWindow<ConfigDataGenerator>();
            window.titleContent = new GUIContent($"Config Data Generator", "Generate all ConfigData script files based on all ConfigSO scriptable objects.");
        }

        /// <summary>
        /// The editor GUI for setting paths and triggering the generation of ConfigData files.
        /// </summary>
        public void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField("Config ScriptableObject Path", new GUIStyle(EditorStyles.boldLabel));
                _configSOPath = EditorGUILayout.TextField(_configSOPath);

                EditorGUILayout.LabelField("Config Data Path", new GUIStyle(EditorStyles.boldLabel));
                _configDataPath = EditorGUILayout.TextField(_configDataPath);

                if (!string.IsNullOrEmpty(_configSOPath))
                {
                    if (GUILayout.Button("Generate"))
                    {
                        try
                        {
                            var configSOBaseList = new List<GameSOBase>();

                            // Get all ConfigSO assets
                            var filePaths = Directory.GetFiles(_configSOPath, "*.asset", SearchOption.AllDirectories);
                            foreach (var filePath in filePaths)
                            {
                                var configSOBase = AssetDatabase.LoadAssetAtPath<GameSOBase>(filePath);
                                if (configSOBase != null)
                                {
                                    configSOBaseList.Add(configSOBase);
                                }
                            }

                            // Generate ConfigData and ConfigDataExtension for each ConfigSO
                            foreach (var configSOBase in configSOBaseList)
                            {
                                var typeName = configSOBase.GetType().GetFriendlyName();
                                var index = typeName.IndexOf("SO");
                                if (index >= 0)
                                {
                                    var key = typeName.Remove(index);
                                    CreateNewConfigDataFile(key);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogError($"{e.Message}\n{e.StackTrace}");
                            return;
                        }

                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new ConfigData file for a given key (derived from ConfigSO class name).
        /// </summary>
        /// <param name="key">The key used to derive the class name and file paths for the new ConfigData and ConfigDataExtension files.</param>
        private void CreateNewConfigDataFile(string key)
        {
            try
            {
                var soTypeFullName = $"com.hive.projectr.{key}SO";
                var soType = TypeUtil.GetType(soTypeFullName);
                if (soType == null)
                {
                    Logger.LogError($"Invalid so type: {soTypeFullName}");
                    return;
                }

                var soItemTypeFullName = $"com.hive.projectr.{key}SOItem, Assembly-CSharp";
                var soItemType = TypeUtil.GetType(soItemTypeFullName);
                if (soItemType == null)
                {
                    Logger.LogError($"Invalid so item type: {soItemTypeFullName}");
                    return;
                }

                var configTypeName = $"{key}Config";
                var configDataTypeName = $"{key}ConfigData";

                // Generate config data
                var configDataPath = Path.Combine(_configDataPath, $"{key}ConfigData.cs");
                if (File.Exists(configDataPath))
                {
                    File.Delete(configDataPath);
                }
                WriteConfigData(configTypeName, configDataTypeName, soType, soItemType, configDataPath);

                // Generate config data extension
                var configDataExtensionPath = Path.Combine(_configDataPath, $"{key}ConfigDataExtension.cs");
                if (!File.Exists(configDataExtensionPath))
                {
                    WriteConfigDataExtension(configTypeName, configDataTypeName, soType, configDataExtensionPath);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Writes the ConfigData script file based on the provided types.
        /// </summary>
        private void WriteConfigData(string configTypeName, string configDataTypeName, Type soType, Type soItemType, string outputPath)
        {
            var configDataMembersSb = new StringBuilder();

            // Iterate through each public field
            foreach (var property in soItemType.GetPublicInstancePropertiesExcludingObject())
            {
                configDataMembersSb.AppendLine($@"{"\t"}{"\t"}public {property.PropertyType.GetFriendlyName()} {property.Name} => _item.{property.Name};");
            }

            var configDef =
$@"
{"\t"}public partial class {configTypeName} : GameConfigBase
{"\t"}{{
{"\t"}{"\t"}private static {soType.GetFriendlyName()} _so;
{"\t"}{"\t"}private static Dictionary<int, {configDataTypeName}> _dict;

{"\t"}{"\t"}public {configTypeName}({soType.GetFriendlyName()} so)
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}_so = so;
{"\t"}{"\t"}}}

{"\t"}{"\t"}public static {configDataTypeName} GetData(int id)
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}if (_dict.TryGetValue(id, out var data))
{"\t"}{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}{"\t"}return data;
{"\t"}{"\t"}{"\t"}}}
{"\t"}{"\t"}{"\t"}return null;
{"\t"}{"\t"}}}

{"\t"}{"\t"}protected override void OnInit()
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}_dict = new Dictionary<int, {configDataTypeName}>();

{"\t"}{"\t"}{"\t"}for (var i = 0; i < _so.Items.Count; ++i)
{"\t"}{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}{"\t"}var id = i + 1;
{"\t"}{"\t"}{"\t"}{"\t"}if (!_dict.ContainsKey(id))
{"\t"}{"\t"}{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}{"\t"}{"\t"}_dict[id] = new {configDataTypeName}(_so.Items[i]);
{"\t"}{"\t"}{"\t"}{"\t"}}}
{"\t"}{"\t"}{"\t"}{"\t"}else
{"\t"}{"\t"}{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}{"\t"}{"\t"}Logger.LogError($""Duplicate id: {{id}} in {soType.Name}!"");
{"\t"}{"\t"}{"\t"}{"\t"}}}
{"\t"}{"\t"}{"\t"}}}

{"\t"}{"\t"}{"\t"}PostInit();
{"\t"}{"\t"}}}

{"\t"}{"\t"}protected override void OnDispose()
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}_dict.Clear();
{"\t"}{"\t"}{"\t"}_dict = null;
{"\t"}{"\t"}{"\t"}_so = null;

{"\t"}{"\t"}{"\t"}PostDispose();
{"\t"}{"\t"}}}
{"\t"}}}";

            // config data
            var configDataDef =
$@"
{"\t"}public partial class {configDataTypeName} : GameConfigDataBase
{"\t"}{{
{"\t"}{"\t"}private {soItemType.GetFriendlyName()} _item;

{configDataMembersSb}
{"\t"}{"\t"}public {configDataTypeName}({soItemType.GetFriendlyName()} item)
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}_item = item;
{"\t"}{"\t"}}}
{"\t"}}}";

            // Write the config data script file
            var configDataScript = File.ReadAllText(ConfigDataTemplatePath);
            string configDefPattern = @"//Config Def Start.*?//Config Def End";
            string configDataDefPattern = @"//ConfigData Def Start.*?//ConfigData Def End";
            configDataScript = Regex.Replace(configDataScript, configDefPattern, configDef, RegexOptions.Singleline);
            configDataScript = Regex.Replace(configDataScript, configDataDefPattern, configDataDef, RegexOptions.Singleline);

            File.WriteAllText(outputPath, configDataScript);
        }

        /// <summary>
        /// Writes the ConfigDataExtension script file based on the provided configuration names and types.
        /// </summary>
        private void WriteConfigDataExtension(string configTypeName, string configDataTypeName, Type soType, string outputPath)
        {
            var configExtensionDef = $@"
{"\t"}public partial class {configTypeName}
{"\t"}{{
{"\t"}{"\t"}private void PostInit()
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}
{"\t"}{"\t"}}}

{"\t"}{"\t"}private void PostDispose()
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}
{"\t"}{"\t"}}}
{"\t"}}}";

            var configDataExtensionDef = @$"
{"\t"}public partial class {configDataTypeName}
{"\t"}{{
{"\t"}{"\t"}
{"\t"}}}
";

            var extensionScript = File.ReadAllText(ConfigDataExtensionTemplatePath);
            string configExtensionDefPattern = @"//Config Def Start.*?//Config Def End";
            string configDataExtensionDefPattern = @"//ConfigData Def Start.*?//ConfigData Def End";
            extensionScript = Regex.Replace(extensionScript, configExtensionDefPattern, configExtensionDef, RegexOptions.Singleline);
            extensionScript = Regex.Replace(extensionScript, configDataExtensionDefPattern, configDataExtensionDef, RegexOptions.Singleline);
            File.WriteAllText(outputPath, extensionScript);
        }
    }
}