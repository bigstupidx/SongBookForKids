using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomBounds))]
public class CustomBoundsInspector : Editor {

    void OnSceneGUI() {
        SerializedProperty size = serializedObject.FindProperty("size");
        SerializedProperty center = serializedObject.FindProperty("center");
        Vector3 centerPos = (target as CustomBounds).transform.position + center.vector3Value;
        Handles.DrawWireCube(centerPos, size.vector3Value);
    }
}
