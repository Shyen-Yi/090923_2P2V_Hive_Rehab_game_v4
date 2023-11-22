using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace com.hive.projectr
{
    public class ConfigDataGenerator : EditorWindow
    {
        private static readonly string ConfigDataTemplatePath = "Assets/Scripts/Core/Config/ConfigDataTemplate.cs";
        private static readonly string ConfigSOPath = "Assets/Resources/Config";
        private static readonly string ConfigDataPath = "Assets/Scripts/Config/Data";

        private static string _configSOPath = ConfigSOPath;
        private static string _configDataPath = ConfigDataPath;

        [MenuItem("Tools/Hive Toolkit/Config Data Generator"),]
        public static void ShowWindow()
        {
            ConfigDataGenerator window = GetWindow<ConfigDataGenerator>();
            window.titleContent = new GUIContent($"Config Data Generator", "Generate all ConfigData script files based on all ConfigSO scriptable objects.");
        }

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

                            var filePaths = Directory.GetFiles(_configSOPath, "*.asset", SearchOption.AllDirectories);
                            foreach (var filePath in filePaths)
                            {
                                var configSOBase = AssetDatabase.LoadAssetAtPath<GameSOBase>(filePath);
                                if (configSOBase != null)
                                {
                                    configSOBaseList.Add(configSOBase);
                                }
                            }

                            foreach (var configSOBase in configSOBaseList)
                            {
                                var typeName = configSOBase.GetType().Name;
                                
                                var index = typeName.IndexOf("SO");
                                if (index >= 0)
                                {
                                    var key = typeName.Remove(index);
                                    var dataPath = Path.Combine(_configDataPath, $"{key}ConfigData.cs");
                                    if (File.Exists(dataPath))
                                    {
                                        File.Delete(dataPath);
                                    }
                                    CreateNewConfigDataFile(key, configSOBase);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.LogError($"{e.Message}\n{e.StackTrace}");
                            return;
                        }

                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        private void CreateNewConfigDataFile(string key, GameSOBase configSOBase)
        {
            try
            {
                var dataPath = Path.Combine(_configDataPath, $"{key}ConfigData.cs");
                var configTypeName = $"{key}Config";
                var configDataTypeName = $"{key}ConfigData";
                var soTypeName = $"{key}SO";

                // config
                var configMembersSb = new StringBuilder();
                var configDataMembersSb = new StringBuilder();
                Type soType = configSOBase.GetType();

                // Iterate through each public field
                foreach (PropertyInfo property in soType.GetProperties((BindingFlags.Public | BindingFlags.Instance) ^ BindingFlags.Default))
                {
                    configMembersSb.AppendLine($@"{"\t"}{"\t"}public static {property.PropertyType.Name} {property.Name} => _data.{property.Name};");
                    configDataMembersSb.AppendLine($@"{"\t"}{"\t"}public {property.PropertyType.Name} {property.Name} => _so.{property.Name};");
                }

                var configDef = 
$@"
{"\t"}public class {configTypeName} : GameConfigBase
{"\t"}{{
{"\t"}{"\t"}private static {configDataTypeName} _data;

{"\t"}{"\t"}public {configTypeName}({soTypeName} so)
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}_data = new {configDataTypeName}(so);
{"\t"}{"\t"}}}

{configMembersSb}
{"\t"}}}";

                // config data
                var configDataDef =
$@"
{"\t"}public class {configDataTypeName} : GameConfigDataBase
{"\t"}{{
{"\t"}{"\t"}private {soTypeName} _so;

{configDataMembersSb}
{"\t"}{"\t"}public {configDataTypeName}({soTypeName} so)
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}_so = so;
{"\t"}{"\t"}}}
{"\t"}}}";

                var scriptText = File.ReadAllText(ConfigDataTemplatePath);

                string configDefPattern = @"//Config Def Start.*?//Config Def End";
                string configDataDefPattern = @"//ConfigData Def Start.*?//ConfigData Def End";
                scriptText = Regex.Replace(scriptText, configDefPattern, configDef, RegexOptions.Singleline);
                scriptText = Regex.Replace(scriptText, configDataDefPattern, configDataDef, RegexOptions.Singleline);

                File.WriteAllText(dataPath, scriptText);
            }
            catch (Exception e)
            {
                LogHelper.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }
    }
}