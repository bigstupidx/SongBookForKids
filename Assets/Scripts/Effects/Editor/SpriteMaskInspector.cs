using UnityEditor;
using UnityEngine;

namespace Syng {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpriteMask))]
    public class SpriteMaskInspector : Editor {

        private SpriteMask mask;

        void OnEnable() {
            mask = target as SpriteMask;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUI.changed) {
                mask.SetMaterialProperties();
            }
        }
    }
}
