using UnityEditor;
using UnityEngine;

namespace Syng {

    [CustomEditor(typeof(CameraRigPlugin))]
    public class CameraRigPluginInspector : Editor {

        private Transform transform;

        static public void AddPlayModeCallback() {
            // read editor settings
            // EditorApplication.playmodeStateChanged
        }

        void OnEnable() {
            transform = (target as CameraRigPlugin).transform;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("cam"));

            SerializedProperty boundsWidth = serializedObject.FindProperty("boundsWidth");
            EditorGUILayout.PropertyField(boundsWidth, new GUIContent("Bounds Width"));

            SerializedProperty dayNightCycle = serializedObject.FindProperty("dayNightCycle");
            EditorGUILayout.PropertyField(dayNightCycle, new GUIContent("Day/Night Cycle"));

            Vector3 local = Camera.main.transform.localPosition;
            local.x /= boundsWidth.floatValue;
            float newx = EditorGUILayout.Slider("Camera Position", local.x, -0.5f, 0.5f);
            local.x = newx * boundsWidth.floatValue;
            Camera.main.transform.localPosition = local;

            SerializedProperty dayNight = serializedObject.FindProperty("dayNight");
            EditorGUILayout.PropertyField(dayNight, new GUIContent("Day/Night"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onTransition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onCameraMove"));

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void OnSceneGUI() {
            DrawBoundsHandle();
            DrawDayNightCurve();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDayNightCurve() {
            SerializedProperty cycle = serializedObject.FindProperty("dayNightCycle");
            SerializedProperty boundsWidth = serializedObject.FindProperty("boundsWidth");

            float width = boundsWidth.floatValue;
            Vector3 left = transform.localPosition - Vector3.right * width * 0.5f;
            Handles.DrawLine(left, left + Vector3.up * 50f);

            float slen = 2f;
            float segments = width / slen;
            int num = (int)Mathf.Ceil(segments) + 1;
            Vector3[] segPoints = new Vector3[num];

            for (int i=0; i<num; i++) {
                float sample = Mathf.Clamp01(i * slen / width);
                Vector3 y = Vector3.up * cycle.animationCurveValue.Evaluate(sample) * 50f;
                Vector3 x = Vector3.right * slen * i;
                segPoints[i] = left + y + x;
            }

            for (int i=0; i<segPoints.Length-1; i++) {
                Vector3 f = segPoints[i];
                Vector3 t = segPoints[i+1];
                Handles.DrawLine(f, t);
            }

            Vector3 right = left + Vector3.right * width;
            Handles.DrawLine(right, right + Vector3.up * 50f);
        }

        private void DrawBoundsHandle() {
            SerializedProperty boundsWidth = serializedObject.FindProperty("boundsWidth");

            Handles.color = Color.green;
            Vector3 handleRight = Vector3.right * boundsWidth.floatValue * 0.5f;
            Vector3 res = Vector3.zero;

            if (Drag(transform.localPosition + handleRight + res,
                     Color.red,
                     out res)) {
                res -= transform.localPosition;
                float x = (handleRight + res).x;
                boundsWidth.floatValue = Mathf.Clamp(x, 0f, x);
            }

            Handles.color = Color.white;
            Handles.DrawLine(transform.localPosition - handleRight,
                             transform.localPosition + handleRight);
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
