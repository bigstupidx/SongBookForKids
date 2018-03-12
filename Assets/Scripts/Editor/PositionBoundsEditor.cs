using UnityEditor;
using UnityEngine;

namespace Syng {

    [CustomEditor(typeof(PositionBounds))]
    public class PositionBoundsEditor : Editor {

        private Transform transform;

        void OnEnable() {
            transform = (target as PositionBounds).transform;
        }

        public override void OnInspectorGUI() {
            Vector3 extents = bounds.extents;
            extents = EditorGUILayout.Vector3Field("Extents", extents);
            SaveExtents(extents);

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void OnSceneGUI() {
            Vector3 ext = bounds.extents;
            Handles.color = Color.green;
            if (Drag(bounds.extents + transform.position, Color.red, out ext)) {
                SaveExtents(ext - transform.position);
            }

            Handles.color = Color.white;
            Handles.DrawWireCube(transform.position, bounds.extents * 2f);

            serializedObject.ApplyModifiedProperties();
        }

        private void SaveExtents(Vector3 extents) {
            Bounds b = bounds;
            b.extents = Clamp(extents);
            bounds = b;
        }

        private Vector3 Clamp(Vector3 v) {
            v.x = v.x < 0 ? 0 : v.x;
            v.y = v.y < 0 ? 0 : v.y;
            v.z = v.z < 0 ? 0 : v.z;
            return v;
        }

        private Bounds bounds {
            get { return serializedObject.FindProperty("bounds").boundsValue; }
            set { serializedObject.FindProperty("bounds").boundsValue = value; }
        }

        private bool Drag(Vector3 handlePos, Color col, out Vector3 pos) {
            float size = HandleUtility.GetHandleSize(handlePos) * 0.25f;
            HandlesHelper.DragHandleResult res;
            pos = HandlesHelper.DragHandle(handlePos,
                                           size,
                                           Handles.CubeCap,
                                           col,
                                           out res);

            switch (res) {
                case HandlesHelper.DragHandleResult.LMBDrag:
                    return true;
            }
            return false;
        }

    }
}
