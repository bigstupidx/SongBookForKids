using UnityEngine;
using UnityEditor;
using System;

namespace Syng {
    [CustomEditor(typeof(Timer))]
    public class TimerInspector : Editor {

        private Action[]    inspectors;

        void OnEnable() {
            inspectors = new Action[] { InspectorFixed, InspectorRandom };
        }

        public override void OnInspectorGUI() {
            SerializedProperty mode = serializedObject.FindProperty("mode");
            EditorGUILayout.PropertyField(mode);
            inspectors[mode.enumValueIndex]();

            EditorHelpers.PropField(serializedObject, "autoStart");
            EditorHelpers.PropField(serializedObject, "repeatMax");

            EditorGUILayout.HelpBox("Set Max Repeats to 0 or less to allow "+
                                    "the timer to repeat indefinitely",
                                    MessageType.None);

            if (Application.isPlaying) {
                EditorGUILayout.Space();
                EditorHelpers.PropField(serializedObject, "curTime", "Current Time");
                EditorGUILayout.Space();
            }

            EditorHelpers.PropField(serializedObject, "onTimer");

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void InspectorFixed() {
            EditorHelpers.PropField(serializedObject, "fixedInterval", "Interval");
        }

        private void InspectorRandom() {
            EditorHelpers.PropField(serializedObject, "randomMin", "Min");
            EditorHelpers.PropField(serializedObject, "randomMax", "Max");
        }
    }
}
