using UnityEditor;
using UnityEngine;

namespace Syng {
    [CustomEditor(typeof(StarsBehaviour))]
    public class StarsBehaviourInspector : Editor {

        void OnSceneGUI() {
            SerializedProperty stars = serializedObject.FindProperty("stars");
            Vector3 parentPos = (target as StarsBehaviour).transform.position;

            for (int i=0; i<stars.arraySize; i++) {
                SerializedProperty star = stars.GetArrayElementAtIndex(i);
                Vector3 pos = (star.objectReferenceValue as StarBehaviour).transform.position;
                Handles.DrawLine(parentPos, pos);
            }
        }
    }
}
