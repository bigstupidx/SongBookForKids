using UnityEditor;

namespace Syng {

    [CustomEditor(typeof(Anchor))]
    public class AnchorInspector : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            EditorGUILayout.HelpBox("Viewport pos is the distance from the "+
                                    "edge of the screen to the anchor. 0,0 is "+
                                    "is the bottom left of the screen.", MessageType.None);
        }

    }
}
