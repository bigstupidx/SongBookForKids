using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(UIPlugin))]
    public class UIPluginInspector : Editor {

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSwitchedPanel"));

            serializedObject.ApplyModifiedProperties();
        }

    }
}
