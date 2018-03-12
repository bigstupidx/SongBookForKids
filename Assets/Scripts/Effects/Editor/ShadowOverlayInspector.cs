using UnityEditor;
using UnityEngine;

namespace Syng {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ShadowOverlay))]
    public class ShadowOverlayInspector : Editor {

        private ShadowOverlay shadow;

        void OnEnable() {
            shadow = target as ShadowOverlay;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUI.changed) {
                shadow.SetMaterialProperties();
            }
        }
    }
}
