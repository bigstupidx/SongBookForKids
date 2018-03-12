using UnityEngine;
using UnityEditor;

namespace Syng {
    [CustomEditor(typeof(ColorTintChildren))]
    public class ColorTintChildrenInspector : Editor {

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty color = serializedObject.FindProperty("color");
            EditorGUILayout.PropertyField(color);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) {
                ApplyToChildren((target as ColorTintChildren).transform,
                                color.colorValue);
            }

        }

        private void ApplyToChildren(Transform parent, Color color) {
            foreach (Transform child in parent) {
                ColorTint tint = child.gameObject.GetComponent<ColorTint>();
                if (tint == null) {
                    tint = child.gameObject.AddComponent<ColorTint>();
                }

                MeshFilter mf = child.GetComponent<MeshFilter>();
                Mesh original = tint.GetOriginalMesh();

                if (mf.sharedMesh == null && original != null) {
                    mf.sharedMesh = original;
                }

                if (mf.sharedMesh != null && original == null) {
                    tint.SetOriginalMesh(mf.sharedMesh);
                }

                Mesh mesh = Mesh.Instantiate(tint.GetOriginalMesh());

                SerializedObject so = new SerializedObject(tint);
                SerializedProperty col = so.FindProperty("color");
                col.colorValue = color;
                so.ApplyModifiedProperties();

                mf.mesh = tint.SetColors(mesh, tint.GetOriginalMesh().name + "_Colored");

                ApplyToChildren(child, color);
            }
        }
    }
}
