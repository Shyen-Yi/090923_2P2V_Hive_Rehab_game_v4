using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Reflection;

namespace com.hive.projectr
{
    public class ConfigTypeGeneratorDummy : ScriptableObject
    {
        public List<string> typeElements;
    }

    public class ConfigTypeGenerator : EditorWindow
    {
        private static readonly string OutputPath = "Assets/Scripts/Config/Types";

        private Vector2 _scrollPos;
        private string _key;
        private bool _isPrevTypeChecked;
        private bool _isPrevTypeLoaded;
        private bool _isValidated;
        private ConfigTypeGeneratorDummy _dummyData;
        private SerializedObject _serializedDummy;

        [MenuItem("Tools/Hive Toolkit/Config Type Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConfigTypeGenerator>();
            window.titleContent = new GUIContent("Config Type Generator");
        }

        private void OnEnable()
        {
            _dummyData = ScriptableObject.CreateInstance<ConfigTypeGeneratorDummy>();
            _serializedDummy = new SerializedObject(_dummyData);
        }

        private void OnDisable()
        {
            if (_serializedDummy != null)
            {
                _serializedDummy.Dispose();
                _serializedDummy = null;
            }

            if (_dummyData != null)
            {
                DestroyImmediate(_dummyData);
                _dummyData = null;
            }
        }

        public void OnGUI()
        {
            _serializedDummy.Update();
            var elementsProp = _serializedDummy.FindProperty("typeElements");

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Type Key", new GUIStyle(EditorStyles.boldLabel));
            _key = EditorGUILayout.TextField(_key);
            if (EditorGUI.EndChangeCheck())
            {
                elementsProp.arraySize = 0;
                _isPrevTypeChecked = false;
                _isPrevTypeLoaded = false;
                _isValidated = false;
            }

            if (!string.IsNullOrEmpty(_key))
            {
                var typeName = $"{_key}Type";
                var outputFilePath = Path.Combine(OutputPath, $"{typeName}.cs");

                GUI.enabled = false;
                EditorGUILayout.LabelField("Preview Type Name", new GUIStyle(EditorStyles.boldLabel));
                EditorGUILayout.TextField(typeName);
                GUI.enabled = true;

                if (!_isPrevTypeChecked)
                {
                    if (GUILayout.Button(new GUIContent("Check", "Check if type with the same name has been defined")))
                    {
                        _isPrevTypeChecked = true;

                        if (File.Exists(outputFilePath))
                        {
                            _isPrevTypeLoaded = false;
                        }
                        else
                        {
                            _isPrevTypeLoaded = true;
                        }
                    }
                }
                else
                {
                    if (!_isPrevTypeLoaded)
                    {
                        if (GUILayout.Button("Load"))
                        {
                            var fullName = $"com.hive.projectr.{typeName}";
                            var type = TypeUtil.GetType(fullName);
                            if (type != null)
                            {
                                var valueNames = Enum.GetNames(type);
                                for (var i = 0; i < valueNames.Length; ++i)
                                {
                                    elementsProp.InsertArrayElementAtIndex(i);
                                    var elementProp = elementsProp.GetArrayElementAtIndex(i);
                                    if (elementProp.propertyType == SerializedPropertyType.String)
                                    {
                                        elementProp.stringValue = valueNames[i];
                                    }
                                }

                                _isPrevTypeLoaded = true;
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
                        EditorGUILayout.PropertyField(elementsProp);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _isValidated = false;
                        }

                        if (elementsProp.arraySize > 0)
                        {
                            if (!_isValidated)
                            {
                                if (GUILayout.Button("Validate"))
                                {
                                    var definedElementTypeNames = new HashSet<string>();
                                    for (var i = elementsProp.arraySize - 1; i >= 0; --i)
                                    {
                                        var elementTypeName = elementsProp.GetArrayElementAtIndex(i).stringValue;
                                        if (string.IsNullOrEmpty(elementTypeName))
                                        {
                                            elementsProp.DeleteArrayElementAtIndex(i);
                                        }
                                        else if (definedElementTypeNames.Contains(elementTypeName))
                                        {
                                            elementsProp.DeleteArrayElementAtIndex(i);
                                        }
                                        else
                                        {
                                            definedElementTypeNames.Add(elementTypeName);
                                        }
                                    }

                                    _isValidated = true;
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Generate"))
                                {
                                    if (File.Exists(outputFilePath))
                                    {
                                        File.Delete(outputFilePath);
                                    }

                                    var typesSb = new StringBuilder();
                                    for (var i = 0; i < elementsProp.arraySize; ++i)
                                    {
                                        var element = elementsProp.GetArrayElementAtIndex(i).stringValue;
                                        if (i < elementsProp.arraySize - 1)
                                        {
                                            typesSb.AppendLine($"{"\t"}{"\t"}{element},");
                                        }
                                        else
                                        {
                                            typesSb.Append($"{"\t"}{"\t"}{element},");
                                        }
                                    }

                                    var script = $@"
namespace com.hive.projectr
{{
{"\t"}public enum {typeName}
{"\t"}{{
{typesSb}
{"\t"}}}
}}
";
                                    File.WriteAllText(outputFilePath, script);
                                    AssetDatabase.Refresh();
                                }
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            _serializedDummy.ApplyModifiedProperties();
        }
    }
}