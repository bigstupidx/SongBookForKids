using UnityEngine;
using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(SpiderThread))]
    public class SpiderThreadInspector : Editor {

        void OnSceneGUI() {
            SpiderThread st = (target as SpiderThread);
            Transform t = st.transform;
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            Vector3 center = sr.bounds.center;
            Vector3 extents = sr.bounds.extents;

            SerializedProperty zones = serializedObject.FindProperty("zones");

            float zoneHeight = st.GetZoneHeight(sr, zones.intValue);

            for (int i=0; i<zones.intValue+1; i++) {
                Vector3 zonePos = center - extents + (Vector3.up * zoneHeight * i);
                Handles.DrawLine(zonePos, zonePos + Vector3.right);
            }
        }
    }
}
