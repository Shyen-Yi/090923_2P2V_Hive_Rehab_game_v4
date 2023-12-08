
using UnityEditor;

namespace com.hive.projectr
{
	[CustomEditor(typeof(DebugSO))]
	public class DebugSOEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var itemsProp = serializedObject.FindProperty("_items");
			EditorGUILayout.PropertyField(itemsProp);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
