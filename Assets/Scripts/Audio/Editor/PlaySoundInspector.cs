using UnityEngine;
using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(PlaySound))]
    public class PlaySoundInspector : Editor {

        override public void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClips"), true);

            SerializedProperty playOrder = serializedObject.FindProperty("playOrder");
            EditorGUILayout.PropertyField(playOrder);

            if (playOrder.enumValueIndex == 2) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sequence"), true);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("numAudioSources"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("output"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDistance"));

            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI() {
            SerializedProperty max = serializedObject.FindProperty("maxDistance");
            Vector3 pos = (target as PlaySound).transform.position;

            Handles.color = Color.cyan;
            Handles.CircleCap(0, pos, Quaternion.identity, max.floatValue);
        }
    }
}
