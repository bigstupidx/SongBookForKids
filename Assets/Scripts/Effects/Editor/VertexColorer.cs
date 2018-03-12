using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Syng {
    public class VertexColorer : EditorWindow {

        private Color               color;
        private Color               darkColor;
        private Color               mediumColor;
        private List<MeshFilter>    mfs;
        private List<Mesh>          meshes;

        [MenuItem("Window/Vertex Colorer")]
        public static void OpenVertexColorer() {
            VertexColorer window = EditorWindow.GetWindow<VertexColorer>();
            window.titleContent = new GUIContent("Vertex Colorer");
            window.Show();
        }

        void OnEnable() {
            color = Color.white;
            darkColor = Color.white;
            mediumColor = Color.white;
            mfs = new List<MeshFilter>();
            meshes = new List<Mesh>();
            EditorApplication.update += OnUpdate;
        }

        void OnDisable() {
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate() {
            meshes.Clear();
            for (int n=0; n<Selection.objects.Length; n++) {
                UnityEngine.Object obj = Selection.objects[n];
                if (obj is Mesh) {
                    meshes.Add(obj as Mesh);
                }
            }

            mfs.Clear();
            for (int i=0; i<Selection.gameObjects.Length; i++) {
                GameObject go = Selection.gameObjects[i];
                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (mf != null) {
                    mfs.Add(mf);
                }
            }

            if (mfs.Count == 0) {
                color = Color.white;
            }

            if (mfs.Count == 1 &&
                mfs[0].sharedMesh != null &&
                mfs[0].sharedMesh.colors.Length > 0) {
                color = mfs[0].sharedMesh.colors[0];
            }

            Repaint();
        }

        void OnGUI() {
            ColorizeField();
            ColorVariants();

            if (meshes.Count == 0) {
                EditorGUILayout.HelpBox("Select a mesh to enable synchronization of the "+
                                        "entire folder.", MessageType.Info);
                return;
            }

            EditorGUILayout.HelpBox("Syncing copies the UVs, vertices and triangles "+
                                    "from the original meshes to the dark and medium "+
                                    "variants, if there are any.", MessageType.None);

            string dataPath = Application.dataPath.Replace("Assets", "");
            string assetPath = AssetDatabase
                .GetAssetPath(meshes[0])
                .Replace(meshes[0].name + ".asset", "");

            if (GUILayout.Button("Synchronize meshes")) {
                string assetPathSys = dataPath + assetPath;
                string[] files = Directory.GetFiles(assetPathSys, "*.asset");
                Array.Sort(files);

                AssetDatabase.StartAssetEditing();

                for (int i=0; i<files.Length; i++) {
                    string filePath = files[i].Replace(dataPath, "");

                    if (filePath.EndsWith("Dark.asset", StringComparison.Ordinal)) {
                        string parentFile = files[i-1].Replace(dataPath, "");
                        CopyUVs(parentFile, filePath);
                        continue;
                    }

                    if (filePath.EndsWith("Medium.asset", StringComparison.Ordinal)) {
                        string parentFile = files[i-2].Replace(dataPath, "");
                        CopyUVs(parentFile, filePath);
                        continue;
                    }
                }

                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void CopyUVs(string fileFrom, string fileTo) {
            Mesh fTo = AssetDatabase.LoadAssetAtPath<Mesh>(fileTo);
            Mesh fFrom = AssetDatabase.LoadAssetAtPath<Mesh>(fileFrom);

            int[] triangles = new int[fFrom.triangles.Length];
            for (int t=0; t<triangles.Length; t++) {
                triangles[t] = fFrom.triangles[t];
            }
            fTo.triangles = triangles;

            Vector3[] vertices = new Vector3[fFrom.vertices.Length];
            for (int v=0; v<vertices.Length; v++) {
                vertices[v] = fFrom.vertices[v];
            }
            fTo.vertices = vertices;

            Vector2[] uvs = new Vector2[fFrom.uv.Length];
            for (int u=0; u<uvs.Length; u++) {
                uvs[u] = fFrom.uv[u];
            }
            fTo.uv = uvs;
        }

        private void CreateAndColorize(Mesh mesh, Color color, string suffix) {
            string meshName = mesh.name + suffix;
            string meshPath = AssetDatabase.GetAssetPath(mesh).Replace(".asset", suffix + ".asset");
            bool create = false;
            Mesh colorizedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (colorizedMesh == null) {
                create = true;
                colorizedMesh = Mesh.Instantiate(mesh);
            }
            colorizedMesh = SetColors(colorizedMesh, color, meshName);
            if (create) {
                AssetDatabase.CreateAsset(colorizedMesh, meshPath);
            }
        }

        private void ColorVariants() {
            if (meshes.Count == 0) {
                return;
            }

            mediumColor = EditorGUILayout.ColorField("Medium", mediumColor);
            darkColor = EditorGUILayout.ColorField("Dark", darkColor);

            if (GUILayout.Button("Make color variants")) {

                AssetDatabase.StartAssetEditing();

                for (int i=0; i<meshes.Count; i++) {
                    CreateAndColorize(meshes[i], mediumColor, "Medium");
                    CreateAndColorize(meshes[i], darkColor, "Dark");
                }

                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void ColorizeField() {
            if (mfs.Count == 0) {
                return;
            }

            Color newColor = EditorGUILayout.ColorField("Set color", color);
            EditorGUILayout.Space();

            if (!GUI.changed) {
                return;
            }

            color = newColor;
            for (int i=0; i<mfs.Count; i++) {
                SetColors(mfs[i]);
            }
        }

        private void SetColors(MeshFilter mf) {
            if (mf.sharedMesh == null) {
                return;
            }
            Mesh mesh = Mesh.Instantiate(mf.sharedMesh);
            mf.mesh = SetColors(mesh, color, mf.sharedMesh.name);
        }

        private Mesh SetColors(Mesh mesh, Color color, string name) {
            mesh.name = name;
            Vector3[] vertices = mesh.vertices;
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                colors[i] = color;
            }
            mesh.colors = colors;
            return mesh;
        }
    }
}
