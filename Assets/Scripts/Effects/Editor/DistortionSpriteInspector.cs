using UnityEditor;
using UnityEngine;

namespace Syng {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(DistortionSprite))]
    public class DistortionSpriteInspector : Editor {

        private DistortionSprite distortion;

        void OnEnable() {
            distortion = target as DistortionSprite;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUI.changed) {
                distortion.SetMaterialProperties();
            }
        }
    }
}
