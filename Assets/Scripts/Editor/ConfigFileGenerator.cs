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
    /// @ingroup Core
    /// @enum ConfigHeaderType
    /// @brief Defines the supported data types for configuration headers.
    /// 
    /// This enum defines the different types of data that can be used as configuration header types. It supports primitive types like
    /// `Int32`, `Boolean`, `String`, as well as more complex types like `AnimationCurve`, `Vector2Int`, and `AssetReference`. 
    /// These types are used to define the type of each configuration field when generating config files.
    public enum ConfigHeaderType
    {
        Enum = 0,
        Int16 = 1,
        Int32 = 2,
        Int64 = 3,
        Boolean = 4,
        String = 5,
        DateTime = 6,
        Char = 7,
        Single = 8,
        Double = 9,
        Decimal = 10,
        Byte = 11,
        SByte = 12,
        UInt16 = 13,
        UInt32 = 14,
        UInt64 = 15,
        AnimationCurve = 16,
        Vector2Int = 17,
        AssetReference = 18,
    }

    /// @ingroup Core
    /// @enum ConfigHeaderTypeNamespace
    /// @brief Defines the namespaces associated with configuration header types.
    /// 
    /// This enum defines the namespaces for different types used in configuration headers. It allows the system to categorize
    /// types based on their namespace, ensuring that the correct namespace is applied when generating code for configuration files.
    public enum ConfigHeaderTypeNamespace
    {
        System,         ///< The System namespace (for types like `Int32`, `String`, etc.).
        ProjectR,       ///< The ProjectR namespace (for custom types specific to the project).
        UnityEngine,    ///< The UnityEngine namespace (for types like `Vector3`, `AnimationCurve`, etc.).
        AddressableAssets ///< The AddressableAssets namespace (for asset references).
    }

    /// @ingroup Core
    /// @class ConfigHeader
    /// @brief Represents a configuration header that defines the structure of a configuration field.
    /// 
    /// The `ConfigHeader` class defines the properties of a configuration field, including its name, type, namespace, and optional
    /// type name if the field is an enum. It is used to describe the structure of fields in a configuration, and it is serialized
    /// to support the generation of configuration files.
    [Serializable]
    public class ConfigHeader
    {
        /// <summary>
        /// The name of the configuration field.
        /// </summary>
        public string name;

        /// <summary>
        /// The type of the configuration field, as defined by the `ConfigHeaderType` enum.
        /// </summary>
        public ConfigHeaderType type;

        /// <summary>
        /// The namespace of the configuration field type, as defined by the `ConfigHeaderTypeNamespace` enum.
        /// </summary>
        public ConfigHeaderTypeNamespace typeNamespace;

        /// <summary>
        /// The name of the enum type, if the configuration field is of an enum type.
        /// </summary>
        public string enumTypeName;
    }

    /// @ingroup Editor
    /// @class ConfigFileGeneratorDummy
    /// @brief A temporary ScriptableObject used for serializing, testing and setting up configuration headers in the Unity editor.
    /// 
    /// The `ConfigFileGeneratorDummy` class is a ScriptableObject used in the Unity editor for testing the generation of config files.
    /// It contains a list of `ConfigHeader` objects that define the structure of a configuration file, which can be serialized
    /// and used to generate or validate configuration files in the editor.
    public class ConfigFileGeneratorDummy : ScriptableObject
    {
        /// <summary>
        /// A list of configuration headers that define the structure of the configuration file.
        /// </summary>
        public List<ConfigHeader> headers = new List<ConfigHeader>();
    }

    /// @ingroup Editor
    /// @class ConfigFileGenerator
    /// @brief A Unity editor tool that creates or replaces ScriptableObject configuration files based on user input.
    /// 
    /// The `ConfigFileGenerator` class is a custom Unity editor tool that assists in generating or replacing ScriptableObject configuration
    /// files. It provides an interface for entering the key, checking if a file already exists, loading existing configuration data, 
    /// validating header data, and generating new config files. The tool allows for seamless integration of configuration files into
    /// Unity projects by automating the creation and modification of script files.
    public class ConfigFileGenerator : EditorWindow
    {
        private string _key;
        private SerializedObject _serializedDummyData;
        private ConfigFileGeneratorDummy _dummyData;
        private Vector2 _scrollPos;

        private bool _isPrevFileChecked;
        private bool _isPrevFileLoaded;
        private bool _isValidated;

        private static readonly string OutputPath = "Assets/Scripts/Config/ScriptableObjects";
        private static readonly string TemplatePath = "Assets/Scripts/Config/ScriptableObjects/Template.cs";
        private static readonly string OutputEditorPath = "Assets/Scripts/Config/ScriptableObjects/Editor";

        /// <summary>
        /// Creates and shows the ConfigFileGenerator window in the Unity Editor.
        /// </summary>
        [MenuItem("Tools/Hive Toolkit/Config File Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConfigFileGenerator>();
            window.titleContent = new GUIContent("Config File Generator", "Create a new or replace the existing config SO");
        }

        /// <summary>
        /// Initializes the ConfigFileGenerator window with default data.
        /// </summary>
        private void OnEnable()
        {
            _dummyData = ScriptableObject.CreateInstance<ConfigFileGeneratorDummy>();
            _serializedDummyData = new SerializedObject(_dummyData);
        }

        /// <summary>
        /// The GUI for the ConfigFileGenerator, allowing users to input the SO key, check if files exist, validate headers, and generate the config file.
        /// </summary>
        public void OnGUI()
        {
            _serializedDummyData.Update();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var headersProp = _serializedDummyData.FindProperty("headers");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("SO Key", new GUIStyle(EditorStyles.boldLabel));
            _key = EditorGUILayout.TextField(_key);
            if (EditorGUI.EndChangeCheck())
            {
                headersProp.arraySize = 0;
                _isPrevFileChecked = false;
                _isPrevFileLoaded = false;
                _isValidated = false;
            }

            if (!string.IsNullOrEmpty(_key))
            {
                var fileName = $"{_key}SO";
                var targetPath = Path.Combine(OutputPath, $"{fileName}.cs");

                EditorGUILayout.LabelField(new GUIContent("Preview Config File Name"), new GUIStyle(EditorStyles.boldLabel));
                GUI.enabled = false;
                EditorGUILayout.TextField(fileName);
                GUI.enabled = true;

                if (!_isPrevFileChecked)
                {
                    if (GUILayout.Button(new GUIContent("Check", "Check if config file with the same name has been created")))
                    {
                        _isPrevFileChecked = true;

                        if (File.Exists(targetPath))
                        {
                            _isPrevFileLoaded = false;
                        }
                        else
                        {
                            _isPrevFileLoaded = true;
                        }
                    }
                }
                else
                {
                    if (!_isPrevFileLoaded)
                    {
                        if (GUILayout.Button("Load"))
                        {
                            var fullName = $"com.hive.projectr.{fileName}Item";
                            var type = TypeUtil.GetType(fullName);
                            if (type != null)
                            {

                                var index = 0;
                                foreach (var property in ReflectionUtil.GetPublicInstancePropertiesExcludingObject(type))
                                {
                                    headersProp.InsertArrayElementAtIndex(index);
                                    var elementProp = headersProp.GetArrayElementAtIndex(index);

                                    var elementType = property.PropertyType;
                                    var elementName = property.Name;

                                    // name
                                    var elementNameProp = elementProp.FindPropertyRelative("name");
                                    if (elementNameProp.propertyType == SerializedPropertyType.String)
                                    {
                                        elementNameProp.stringValue = elementName;
                                    }

                                    // type + enum type name
                                    var elementTypeProp = elementProp.FindPropertyRelative("type");
                                    var elementEnumTypeNameProp = elementProp.FindPropertyRelative("enumTypeName");
                                    if (elementType.IsEnum)
                                    {
                                        if (elementTypeProp.propertyType == SerializedPropertyType.Enum)
                                        {
                                            elementTypeProp.enumValueFlag = (int)ConfigHeaderType.Enum;
                                        }
                                        if (elementEnumTypeNameProp.propertyType == SerializedPropertyType.String)
                                        {
                                            elementEnumTypeNameProp.stringValue = elementType.GetFriendlyName();
                                        }
                                    }
                                    else
                                    {
                                        if (Enum.TryParse<ConfigHeaderType>(elementType.GetFriendlyName(), out var headerType))
                                        {
                                            if (elementTypeProp.propertyType == SerializedPropertyType.Enum)
                                            {
                                                elementTypeProp.enumValueFlag = (int)headerType;
                                            }
                                        }
                                        else
                                        {
                                            Logger.LogError($"Undefined element type name: {elementType.GetFriendlyName()}");
                                        }
                                        
                                        if (elementEnumTypeNameProp.propertyType == SerializedPropertyType.String)
                                        {
                                            elementEnumTypeNameProp.stringValue = null;
                                        }
                                    }

                                    // type namespace
                                    var namespaceType = GetNamespace(elementType.Namespace);
                                    var elementTypeNamespaceProp = elementProp.FindPropertyRelative("typeNamespace");
                                    if (elementTypeNamespaceProp.propertyType == SerializedPropertyType.Enum)
                                    {
                                        elementTypeNamespaceProp.enumValueFlag = (int)namespaceType;
                                    }

                                    ++index;
                                }

                                _isPrevFileLoaded = true;
                            }
                            else
                            {
                                Logger.LogError($"Cannot find type: {fullName}");
                            }
                        }
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.LabelField(new GUIContent("Headers"), new GUIStyle(EditorStyles.boldLabel));
                        EditorGUILayout.PropertyField(headersProp);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _isValidated = false;
                        }

                        if (headersProp.arraySize > 0)
                        {
                            if (!_isValidated)
                            {
                                if (GUILayout.Button("Validate"))
                                {
                                    var definedElementNames = new HashSet<string>();
                                    for (var i = headersProp.arraySize - 1; i >= 0; --i)
                                    {
                                        var headerProp = headersProp.GetArrayElementAtIndex(i);
                                        var elementName = headerProp.FindPropertyRelative("name").stringValue;
                                        if (string.IsNullOrEmpty(elementName))
                                        {
                                            headersProp.DeleteArrayElementAtIndex(i);
                                        }
                                        else if (definedElementNames.Contains(elementName))
                                        {
                                            headersProp.DeleteArrayElementAtIndex(i);
                                        }
                                        else
                                        {
                                            definedElementNames.Add(elementName);
                                        }
                                    }

                                    _isValidated = true;
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Generate"))
                                {
                                    if (File.Exists(targetPath))
                                    {
                                        if (EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to replace the existing {fileName}?", "Yes", "Rethink"))
                                        {
                                            File.Delete(targetPath);
                                            Generate();
                                        }
                                    }
                                    else
                                    {
                                        Generate();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            _serializedDummyData.ApplyModifiedProperties();
        }

        /// <summary>
        /// Cleans up resources when the window is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (_serializedDummyData != null)
            {
                _serializedDummyData.Dispose();
                _serializedDummyData = null;
            }

            if (_dummyData != null)
            {
                DestroyImmediate(_dummyData);
            }
        }

        /// <summary>
        /// Generates the ScriptableObject configuration file based on the provided data.
        /// </summary>
        private void Generate()
        {
            if (!File.Exists(TemplatePath))
            {
                EditorUtility.DisplayDialog("Error", $"{TemplatePath} doesn't exist!", "Confirm");
                return;
            }

            WriteEditorScript();

            var headersDef = GetHeadersDef();

            var txt = File.ReadAllText(TemplatePath).Replace("Template", _key);
            string headersDefPattern = @"//Headers Start.*?//Headers End";
            txt = Regex.Replace(txt, headersDefPattern, headersDef, RegexOptions.Singleline);

            var targetPath = $"{OutputPath}/{_key}SO.cs";

            using (var s = File.CreateText(targetPath))
            {
                s.Write(txt);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Writes the custom editor script for the generated ScriptableObject.
        /// </summary>
        private void WriteEditorScript()
        {
            var editorFileName = $"{_key}SOEditor.cs";
            var outputEditorPath = Path.Combine(OutputEditorPath, editorFileName);
            if (File.Exists(outputEditorPath))
            {
                File.Delete(outputEditorPath);
            }

            var script = $@"
using UnityEditor;

namespace com.hive.projectr
{{
{"\t"}[CustomEditor(typeof({_key}SO))]
{"\t"}public class {_key}SOEditor : Editor
{"\t"}{{
{"\t"}{"\t"}public override void OnInspectorGUI()
{"\t"}{"\t"}{{
{"\t"}{"\t"}{"\t"}serializedObject.Update();

{"\t"}{"\t"}{"\t"}var itemsProp = serializedObject.FindProperty(""_items"");
{"\t"}{"\t"}{"\t"}EditorGUILayout.PropertyField(itemsProp);

{"\t"}{"\t"}{"\t"}serializedObject.ApplyModifiedProperties();
{"\t"}{"\t"}}}
{"\t"}}}
}}
";
            File.WriteAllText(outputEditorPath, script);
        }

        /// <summary>
        /// Returns a string that represents the header definitions for the generated script.
        /// </summary>
        private string GetHeadersDef()
        {
            var privateSerializedSb = new StringBuilder();
            var publicPropSb = new StringBuilder();

            var headersProp = _serializedDummyData.FindProperty("headers");

            for (var i = 0; i < headersProp.arraySize; ++i)
            {
                var headerProp = headersProp.GetArrayElementAtIndex(i);
                var headerType = (ConfigHeaderType)headerProp.FindPropertyRelative("type").enumValueFlag;

                var typeName = headerType == ConfigHeaderType.Enum
                    ? headerProp.FindPropertyRelative("enumTypeName").stringValue
                    : headerType.ToString();

                var headerName = headerProp.FindPropertyRelative("name").stringValue;
                if (string.IsNullOrEmpty(headerName))
                {
                    Logger.LogError($"headerName is null or empty");
                    continue;
                }

                headerName = headerName.Replace(" ", "");
                var headerNameCamel = StringUtil.ConvertToCamel(headerName);
                var headerNamePascal = StringUtil.ConvertToPascal(headerName);

                var privateSerializedStr = @$"{"\t"}{"\t"}[SerializeField] private {typeName} {headerNameCamel};";
                var publicPropStr = @$"{"\t"}{"\t"}public {typeName} {headerNamePascal} => {headerNameCamel};";
                if (i < headersProp.arraySize - 1)
                {
                    privateSerializedSb.AppendLine(privateSerializedStr);
                    publicPropSb.AppendLine(publicPropStr);
                }
                else
                {
                    privateSerializedSb.Append(privateSerializedStr);
                    publicPropSb.Append(publicPropStr);
                }
            }

            return $@"
{privateSerializedSb}

{publicPropSb}";
        }

        /// <summary>
        /// Gets the appropriate namespace based on the given string value.
        /// </summary>
        private string GetNamespace(ConfigHeaderTypeNamespace type)
        {
            switch (type)
            {
                case ConfigHeaderTypeNamespace.System:
                    return "System";
                case ConfigHeaderTypeNamespace.ProjectR:
                    return "com.hive.projectr";
                case ConfigHeaderTypeNamespace.UnityEngine:
                    return "UnityEngine";
                case ConfigHeaderTypeNamespace.AddressableAssets:
                    return "UnityEngine.AddressableAssets";
                default:
                    Logger.LogError($"Undefined ConfigHeaderTypeNamespace {type}");
                    return "System";
            }
        }

        /// <summary>
        /// Converts a namespace string to a corresponding ConfigHeaderTypeNamespace value.
        /// </summary>
        private ConfigHeaderTypeNamespace GetNamespace(string nameSpace)
        {
            if (nameSpace == "System")
                return ConfigHeaderTypeNamespace.System;
            if (nameSpace == "com.hive.projectr")
                return ConfigHeaderTypeNamespace.ProjectR;
            if (nameSpace == "UnityEngine")
                return ConfigHeaderTypeNamespace.UnityEngine;
            if (nameSpace == "UnityEngine.AddressableAssets")
                return ConfigHeaderTypeNamespace.AddressableAssets;

            Logger.LogError($"Undefined namespace: {nameSpace}");
            return ConfigHeaderTypeNamespace.System;
        }
    }
}