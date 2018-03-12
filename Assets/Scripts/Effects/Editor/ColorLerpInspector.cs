using UnityEditor;
using UnityEngine;

namespace Syng {
    [CustomEditor(typeof(ColorLerp))]
    public class ColorLerpInspector : Editor {

        private float lerpValue = 0f;

        override public void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty colFrom = serializedObject.FindProperty("colorFrom");
            EditorGUILayout.PropertyField(colFrom, new GUIContent("From Color"));
            SerializedProperty colTo = serializedObject.FindProperty("colorTo");
            EditorGUILayout.PropertyField(colTo, new GUIContent("To Color"));

            ColorLerp lerp = target as ColorLerp;
            MeshRenderer mr = lerp.GetComponent<MeshRenderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            lerpValue = EditorGUILayout.Slider(lerpValue, 0f, 1f);
            lerp.SetColor(lerp.GetColorAt(lerpValue), mr, block);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
