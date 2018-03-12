using UnityEditor;
using UnityEngine;

namespace Syng {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(GradientOverlay))]
    public class GradientOverlayInspector : Editor {

        private GradientOverlay gradient;

        void OnEnable() {
            gradient = target as GradientOverlay;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUI.changed) {
                gradient.SetMaterialProperties();
            }
        }
    }
}
