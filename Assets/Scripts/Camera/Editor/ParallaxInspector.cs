using UnityEngine;
using UnityEditor;

namespace Syng {

    [InitializeOnLoad]
    public class ParallaxHelper {

        static ParallaxHelper() {
            EditorApplication.playmodeStateChanged += OnPlayChange;
        }

        static private void OnPlayChange() {
            if (EditorApplication.isPlaying &&
                EditorApplication.isPlayingOrWillChangePlaymode) {
                Reindex();
            }
        }

        static public void Reindex(Parallax parallax = null) {
            if (parallax == null) {
                parallax = GameObject.FindObjectOfType<Parallax>();
                if (parallax == null) {
                    return;
                }
            }
            parallax.IndexParallaxElements();
        }
    }

    [CustomEditor(typeof(Parallax))]
    public class ParallaxInspector : Editor {

        private Camera              cam;
        private Gradient            gradient;
        private Parallax            parallax;
        private float               opacity = 0.05f;

        void OnEnable() {
            parallax = target as Parallax;
            cam = parallax.GetComponent<Camera>();
            FillGradient();
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;
        }

        void OnDisable() {
            SceneView.onSceneGUIDelegate -= OnScene;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("zones"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speedCurve"));

            SerializedProperty tag = serializedObject.FindProperty("tagName");
            tag.stringValue = EditorGUILayout.TagField("Parallax tag", tag.stringValue);

            opacity = EditorGUILayout.Slider("Zone opacity", opacity, 0f, 1f);

            if (GUILayout.Button("Index elements")) {
                ParallaxHelper.Reindex(parallax);
            }

            EditorGUILayout.Space();

            if (GUI.changed) {
                FillGradient();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnScene(SceneView sceneView) {
            Vector3 pxStart = cam.transform.position;
            Vector3 pxEnd = pxStart + Vector3.forward * cam.farClipPlane;
            Handles.DrawLine(pxStart, pxEnd);

            SerializedProperty curve = serializedObject.FindProperty("speedCurve");

            Vector3 newDistance = pxEnd;
            if (Drag(pxEnd, ref newDistance)) {
                cam.farClipPlane = newDistance.z;
                serializedObject.ApplyModifiedProperties();
            }

            int zones = serializedObject.FindProperty("zones").intValue;
            float zoneLen = cam.farClipPlane / zones;

            for (int z=0; z<zones+1; z++) {
                Vector3 zonePos = cam.transform.position + Vector3.forward * z * zoneLen;
                Vector3 camWidth = cam.orthographicSize * cam.aspect * Vector3.right * 1000f;
                Vector3 camHeight = cam.orthographicSize * Vector3.up;
                Vector3[] verts = new Vector3[] {
                    zonePos - camWidth + camHeight,
                    zonePos - camWidth - camHeight,
                    zonePos + camWidth - camHeight,
                    zonePos + camWidth + camHeight,
                };

                Color fill = gradient.Evaluate((float)z / (float)zones);
                Color outline = fill;
                outline.a = 1f;
                Handles.DrawSolidRectangleWithOutline(verts, fill, outline);

                if (z == zones) {
                    continue;
                }

                float samplePoint = parallax.GetZoneSamplePoint(z);
                float zoneValue = curve.animationCurveValue.Evaluate(samplePoint);
                Handles.Label(zonePos + Vector3.forward * zoneLen * 0.5f,
                              zoneValue.ToString());
            }
        }

        private bool Drag(Vector3 handlePos, ref Vector3 pos) {
            float size = HandleUtility.GetHandleSize(handlePos) * 0.25f;
            HandlesHelper.DragHandleResult res;
            pos = HandlesHelper.DragHandle(handlePos,
                                           size,
                                           Handles.ConeCap,
                                           Color.white,
                                           out res);

            switch (res) {
                case HandlesHelper.DragHandleResult.LMBDrag:
                    return true;
            }
            return false;
        }

        private void FillGradient() {
            gradient = new Gradient();

            GradientColorKey[] gck = new GradientColorKey[3];
            gck[0].color = Color.yellow;
            gck[0].time = 0.0F;
            gck[1].color = Color.red;
            gck[1].time = 0.5F;
            gck[2].color = Color.red + Color.blue;
            gck[2].time = 1.0F;

            GradientAlphaKey[] gak;
            gak = new GradientAlphaKey[2];
            gak[0].alpha = opacity;
            gak[0].time = 0.0F;
            gak[1].alpha = opacity;
            gak[1].time = 1.0F;

            gradient.SetKeys(gck, gak);
        }
    }
}
