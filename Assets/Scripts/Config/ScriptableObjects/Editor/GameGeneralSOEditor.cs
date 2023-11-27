
using UnityEditor;

namespace com.hive.projectr
{
	[CustomEditor(typeof(GameGeneralSO))]
	public class GameGeneralSOEditor : Editor
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
