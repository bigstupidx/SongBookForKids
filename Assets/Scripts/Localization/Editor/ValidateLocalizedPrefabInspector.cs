using UnityEngine;
using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(ValidateLocalizedPrefab))]
    public class ValidateLocalizedPrefabInspector : Editor {

        private LocalizationPlugin loc;

        void OnEnable() {
            loc = (LocalizationPlugin)GameObject.FindObjectOfType(typeof(LocalizationPlugin));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty prefab = serializedObject.FindProperty("prefab");
            int newLang = EditorGUILayout.Popup("Prefabs", prefab.intValue, loc.GetPrefabKeys());
            if (newLang != prefab.intValue) {
                prefab.intValue = newLang;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onValid"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
