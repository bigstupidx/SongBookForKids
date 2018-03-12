using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Syng {

    public class DropArea<T> where T : Object {

        private Texture2D   dropboxBG;
        private GUIStyle    style;
        private string      label;

        public DropArea(string label) {
            this.label = label;

            dropboxBG = new Texture2D(1, 1);
            float c = 76f / 255f;
            dropboxBG.SetPixel(0, 0, new Color(c, c, c));
            dropboxBG.Apply();

            style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            style.normal.background = dropboxBG;
        }

        public bool Draw(out List<T> assets) {
            assets = new List<T>();
            Event evt = Event.current;
            Rect rect = GUILayoutUtility.GetRect(0.0f,
                                                 50.0f,
                                                 GUILayout.ExpandWidth(true));
            GUI.Box(rect, label, style);
            bool didDrag = false;

            switch (evt.type) {
                case EventType.DragUpdated:
                    if (!rect.Contains(evt.mousePosition)) {
                        return false;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;

                case EventType.DragPerform:
                    D.Log(evt.type);
                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged in DragAndDrop.objectReferences) {
                        if (dragged is Texture2D) {
                            string path = AssetDatabase.GetAssetPath(dragged);
                            Object[] loaded = AssetDatabase.LoadAllAssetsAtPath(path);
                            if (loaded == null) {
                                return false;
                            }
                            foreach (Object asset in loaded) {
                                if (asset.GetType() != typeof(T)) {
                                    continue;
                                }
                                assets.Add((T)asset);
                                didDrag = true;
                                D.Log(asset);
                            }
                        }
                    }

                    break;
            }

            return didDrag;
        }
    }

    // [CustomEditor(typeof(Flipbook))]
    public class FlipbookInspector : Editor {

        private SerializedProperty  sprites;
        private DropArea<Sprite>    drop;

        void OnEnable() {
            sprites = serializedObject.FindProperty("sprites");
            drop = new DropArea<Sprite>("Drop Sprites here");
        }

        public override void OnInspectorGUI() {
            List<Sprite> droppedSprites;
            drop.Draw(out droppedSprites);

            for (int i=0; i<sprites.arraySize; i++) {
                Sprite sprite = sprites.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
                EditorGUILayout.LabelField(sprite.name);
            }

            if (GUILayout.Button("Clear sprites")) {
                sprites.ClearArray();
            }

            if (droppedSprites.Count > 0) {
                for (int i=0; i<sprites.arraySize; i++) {
                    Sprite sprite = sprites.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
                    int j = droppedSprites.IndexOf(sprite);
                    if (j >= 0) {
                        droppedSprites.RemoveAt(j);
                    }
                }

                List<Sprite> existing = new List<Sprite>(sprites.arraySize);
                for (int i=0; i<sprites.arraySize; i++) {
                    existing.Add(sprites.GetArrayElementAtIndex(i).objectReferenceValue as Sprite);
                }

                droppedSprites.InsertRange(0, existing);
                sprites.ClearArray();
                for (int i=0; i<droppedSprites.Count; i++) {
                    sprites.InsertArrayElementAtIndex(i);
                    SerializedProperty prop = sprites.GetArrayElementAtIndex(i);
                    prop.objectReferenceValue = droppedSprites[i];
                }

                serializedObject.ApplyModifiedProperties();
            }

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

    }
}
