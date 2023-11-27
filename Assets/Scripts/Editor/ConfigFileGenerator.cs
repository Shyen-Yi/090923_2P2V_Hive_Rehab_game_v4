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
    public enum ConfigHeaderType
    {
        Enum,
        Int16,
        Int32,
        Int64,
        Boolean,
        String,
        DateTime,
        Char,
        Single,
        Double,
        Decimal,
        Byte,
        SByte,
        UInt16,
        UInt32,
        UInt64,
    }

    public enum ConfigHeaderTypeNamespace
    {
        System,
        ProjectR,
    }

    [Serializable]
    public class ConfigHeader
    {
        public string name;
        public ConfigHeaderType type;
        public ConfigHeaderTypeNamespace typeNamespace;
        public string enumTypeName;
    }

    public class ConfigFileGeneratorDummy : ScriptableObject
    {
        public List<ConfigHeader> headers = new List<ConfigHeader>();
    }

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

        [MenuItem("Tools/Hive Toolkit/Config File Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConfigFileGenerator>();
            window.titleContent = new GUIContent("Config File Generator", "Create a new or replace the existing config SO");
        }

        private void OnEnable()
        {
            _dummyData = ScriptableObject.CreateInstance<ConfigFileGeneratorDummy>();
            _serializedDummyData = new SerializedObject(_dummyData);
        }

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

        private string GetHeadersDef()
        {
            var privateSerializedSb = new StringBuilder();
            var publicPropSb = new StringBuilder();

            var headersProp = _serializedDummyData.FindProperty("headers");

            for (var i = 0; i < headersProp.arraySize; ++i)
            {
                var headerProp = headersProp.GetArrayElementAtIndex(i);
                var headerType = (ConfigHeaderType)headerProp.FindPropertyRelative("type").enumValueFlag;

                var typeNamespace = GetNamespace((ConfigHeaderTypeNamespace)headerProp.FindPropertyRelative("typeNamespace").enumValueFlag);
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

        private string GetNamespace(ConfigHeaderTypeNamespace type)
        {
            switch (type)
            {
                case ConfigHeaderTypeNamespace.System:
                    return "System";
                case ConfigHeaderTypeNamespace.ProjectR:
                    return "com.hive.projectr";
                default:
                    Logger.LogError($"Undefined ConfigHeaderTypeNamespace {type}");
                    return "System";
            }
        }

        private ConfigHeaderTypeNamespace GetNamespace(string nameSpace)
        {
            if (nameSpace == "System")
                return ConfigHeaderTypeNamespace.System;
            if (nameSpace == "com.hive.projectr")
                return ConfigHeaderTypeNamespace.ProjectR;

            Logger.LogError($"Undefined namespace: {nameSpace}");
            return ConfigHeaderTypeNamespace.System;
        }
    }
}