using UnityEditor;
using UnityEngine;

namespace Syng {
    static public class EditorHelpers {

        static public void PropField(SerializedObject so, string propName, string label = null) {
            if (label == null) {
                EditorGUILayout.PropertyField(so.FindProperty(propName));
            } else {
                EditorGUILayout.PropertyField(so.FindProperty(propName), new GUIContent(label));
            }
        }

        static public void PropField(SerializedProperty prop, string label = null) {
            if (label == null) {
                EditorGUILayout.PropertyField(prop);
            } else {
                EditorGUILayout.PropertyField(prop, new GUIContent(label));
            }
        }
    }
}
