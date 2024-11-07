
using UnityEditor;

namespace com.hive.projectr
{
	[CustomEditor(typeof(MailSO))]
	public class MailSOEditor : Editor
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
