using UnityEditor;
using UnityEngine;

namespace Syng {
    public class TintColors : EditorWindow {

        [MenuItem("Window/Tint Colors")]
        public static void OpenTintColors() {
            TintColors window = EditorWindow.GetWindow<TintColors>();
            window.titleContent = new GUIContent("Tint Colors");
            window.Show();
        }

        void OnGUI() {
            if (GUILayout.Button("Force Update All")) {
                ForceUpdateAll();
            }
        }

        private void ForceUpdateAll() {
            ColorTint[] tints = GameObject.FindObjectsOfType<ColorTint>();
            foreach (ColorTint ct in tints) {
                ct.ForceUpdate();
            }
        }
    }
}
