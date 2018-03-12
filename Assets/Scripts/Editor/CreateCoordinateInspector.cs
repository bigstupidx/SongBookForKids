using UnityEngine;
using UnityEditor;
using System;

namespace Syng {
    [CustomEditor(typeof(CreateCoordinate))]
    public class CreateCoordinateInspector : Editor {

        private CreateCoordinate    coord;
        private Action[]            inspectors;
        private Action[]            handles;
        private Vector3             pivot;
        private int                 handle;
        private bool                toggleAxisEnabled;
        private bool[]              toggleAxis;

        void OnEnable() {
            coord = target as CreateCoordinate;
            inspectors = new Action[] {
                InspectorFixed,
                InspectorRandomUnit,
                InspectorRandomBoundaries
            };
            handles = new Action[] {
                NoHandles,
                HandlesBoundaries
            };
            handle = 0;
            toggleAxis = new bool[] { true, true, true };
        }

        public override void OnInspectorGUI() {
            handle = 0;
            EditorHelpers.PropField(serializedObject, "mode");
            inspectors[serializedObject.FindProperty("mode").enumValueIndex]();

            EditorHelpers.PropField(serializedObject, "onCoordCreated");

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void OnSceneGUI() {
            handles[handle]();
        }

        private void InspectorFixed() {
            EditorHelpers.PropField(serializedObject, "fixedCoordinate");
        }

        private void InspectorRandomUnit() {
            SerializedProperty ua = serializedObject.FindProperty("unitAxis");
            toggleAxis[0] = ua.vector3Value.x > .5f;
            toggleAxis[1] = ua.vector3Value.y > .5f;
            toggleAxis[2] = ua.vector3Value.z > .5f;

            toggleAxisEnabled = EditorGUILayout.BeginToggleGroup("Limit Coordinate To Axis", toggleAxisEnabled);
            toggleAxis[0] = EditorGUILayout.Toggle("X", toggleAxis[0]);
            toggleAxis[1] = EditorGUILayout.Toggle("Y", toggleAxis[1]);
            toggleAxis[2] = EditorGUILayout.Toggle("Z", toggleAxis[2]);
            EditorGUILayout.EndToggleGroup();

            ua.vector3Value = new Vector3(toggleAxis[0] ? 1f : 0f,
                                          toggleAxis[1] ? 1f : 0f,
                                          toggleAxis[2] ? 1f : 0f);

            EditorGUILayout.HelpBox("Use this to limit the coordinate to some "+
                                    "axis. All axis will be used when no "+
                                    "limits are set.",
                                    MessageType.Info);
        }

        private void InspectorRandomBoundaries() {
            handle = 1;
            string worldLocalLabel = IsWorldSpace() ? "World Space" : "Local Space";
            float val = IsWorldSpace() ? 1f : 0f;
            bool world = EditorGUILayout.Slider(worldLocalLabel, val, 0f, 1f) > .5f;
            if (world != IsWorldSpace()) {
                IsWorldSpace(world);
                SceneView.RepaintAll();
            }
            pivot = IsWorldSpace() ? Vector3.zero : coord.transform.position;

            EditorHelpers.PropField(serializedObject, "boundaries");
        }

        private void NoHandles() {}

        private void HandlesBoundaries() {
            SerializedProperty boundaries = serializedObject.FindProperty("boundaries");
            Rect bRect = boundaries.rectValue;

            Vector3 tl = new Vector3(bRect.x, bRect.y, 0f);
            if (!IsWorldSpace()) {
                tl.x += coord.transform.position.x;
                tl.y += coord.transform.position.y;
            }

            Vector3 tr = tl + Vector3.right * bRect.width;
            Vector3 bl = tl + Vector3.down * bRect.height;
            Vector3 br = tl + Vector3.right * bRect.width + Vector3.down * bRect.height;

            Handles.DrawLine(tl, tr);
            Handles.DrawLine(tl, bl);
            Handles.DrawLine(tr, br);
            Handles.DrawLine(bl, br);

            Vector3 handleTop = tl + (tr - tl) * .5f;
            Vector3 handleRight = tr + (br - tr) * .5f;
            Vector3 handleLeft = tl + (bl - tl) * .5f;
            Vector3 handleBottom = bl + (br - bl) * .5f;

            Vector3 newPos = Vector3.zero;
            Handles.color = Color.red;
            if (Drag(handleTop, Color.red, ref newPos)) {
                newPos = Translate(newPos);
                boundaries.rectValue = new Rect(bRect.x, newPos.y, bRect.width, bRect.height);
            }
            Handles.color = Color.white;
            if (Drag(handleRight, Color.white, ref newPos)) {
                newPos = Translate(newPos);
                boundaries.rectValue = new Rect(bRect.x, bRect.y, newPos.x - bRect.x, bRect.height);
            }
            if (Drag(handleLeft, Color.white, ref newPos)) {
                newPos = Translate(newPos);
                boundaries.rectValue = new Rect(newPos.x, bRect.y, bRect.width, bRect.height);
            }
            Handles.color = Color.blue;
            if (Drag(handleBottom, Color.blue, ref newPos)) {
                newPos = Translate(newPos);
                boundaries.rectValue = new Rect(bRect.x, bRect.y, bRect.width, bRect.y - newPos.y);
            }

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private bool IsWorldSpace() {
            return serializedObject.FindProperty("boundariesWorldSpace").boolValue;
        }

        private void IsWorldSpace(bool val) {
            serializedObject.FindProperty("boundariesWorldSpace").boolValue = val;
        }

        private bool Drag(Vector3 handlePos, Color col, ref Vector3 pos) {
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

        private Vector3 Translate(float x, float y, float z) {
            return Translate(new Vector3(x, y, z));
        }

        private Vector3 Translate(Vector3 v3) {
            return v3 - pivot;
        }

    }
}
