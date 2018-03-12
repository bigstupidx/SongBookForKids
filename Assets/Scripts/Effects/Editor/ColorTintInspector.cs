using UnityEngine;
using UnityEditor;

namespace Syng {
    [CustomEditor(typeof(ColorTint))]
    public class ColorTintInspector : Editor {

        private MeshFilter  mf;

        void OnEnable() {
            mf = (target as ColorTint).GetComponent<MeshFilter>();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty originalMesh = serializedObject.FindProperty("originalMesh");
            EditorGUILayout.PropertyField(originalMesh);
            Mesh original = (Mesh)originalMesh.objectReferenceValue;

            if (original == null) {
                return;
            }

            SerializedProperty color = serializedObject.FindProperty("color");
            Color prevColor = color.colorValue;
            EditorGUILayout.PropertyField(color);

            if (GUILayout.Button("Reset")) {
                // This does not work on the first click. Have to click twice
                mf.sharedMesh = (Mesh)originalMesh.objectReferenceValue;
                color.colorValue = Color.white;
            }

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Force Update")) {
                mf.mesh = (target as ColorTint).SetColors(Mesh.Instantiate(original),
                                                          original.name + "_Colored");
            }


            if (prevColor != color.colorValue) {
                mf.mesh = (target as ColorTint).SetColors(Mesh.Instantiate(original),
                                                          original.name + "_Colored");
            }
        }
    }
}
