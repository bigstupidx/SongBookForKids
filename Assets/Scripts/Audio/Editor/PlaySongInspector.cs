using UnityEngine;
using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(PlaySong))]
    public class PlaySongInspector : Editor {

        private LocalizationPlugin loc;

        void OnEnable() {
            loc = (LocalizationPlugin)GameObject.FindObjectOfType(typeof(LocalizationPlugin));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty song = serializedObject.FindProperty("songIndex");
            int newSong = EditorGUILayout.Popup("Songs", song.intValue, loc.GetAudioKeys());
            if (newSong != song.intValue) {
                song.intValue = newSong;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onCanPlay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onCannotPlay"));

            serializedObject.ApplyModifiedProperties();
        }

    }
}
