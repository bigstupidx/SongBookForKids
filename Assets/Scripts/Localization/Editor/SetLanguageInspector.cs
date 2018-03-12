using UnityEngine;
using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(SetLanguage))]
    public class SetLanguageInspector : Editor {

        private LocalizationPlugin loc;

        void OnEnable() {
            loc = (LocalizationPlugin)GameObject.FindObjectOfType(typeof(LocalizationPlugin));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty lang = serializedObject.FindProperty("language");
            int newLang = EditorGUILayout.Popup("Languages", lang.intValue, loc.GetLanguages());
            if (newLang != lang.intValue) {
                lang.intValue = newLang;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSetLanguage"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
