using UnityEngine;
using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(LocalizationPlugin))]
    public class LocalizationPluginInspector : Editor {

        private int lang;
        private string[] languages;

        void OnEnable() {
            lang = 0;
            SerializedProperty langs = serializedObject.FindProperty("languages");
            languages = new string[langs.arraySize];
            for (int i=0; i<langs.arraySize; i++) {
                languages[i] = langs.GetArrayElementAtIndex(i).stringValue;
            }
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            lang = EditorGUILayout.Popup("Languages", lang, languages);
            if (GUILayout.Button("Switch to "+languages[lang])) {
                (target as LocalizationPlugin).LoadLanguage(lang);
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}
