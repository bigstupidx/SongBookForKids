#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(SimpleMeshCombine))]
public class SimpleMeshCombineEditor : Editor {

    private string savedMeshLoc = "Assets/Visuals/Meshes";
    private SimpleMeshCombine smc;

    void OnEnable() {
        smc = (SimpleMeshCombine)target;
    }

    override public void OnInspectorGUI() {
        Info("* All meshes must have same material *");

        if (!smc.combined) {
            GUILayout.Label("Combine all Mesh Renderer enabled meshes");
            if (GUILayout.Button("Combine")) {
                CombineMeshes();
            }
            Toggle("Generate Ligthmap UV's", ref smc.generateLightmapUV);
       } else {
            GUILayout.Label("Decombine all previously combined meshes");
            if (GUILayout.Button("Release")) {
                MeshCombinerCore.EnableRenderers(smc.combinedGameObjects,
                                                 true);
                smc.savedPrefab = false;
                if (smc.combined) {
                    DestroyImmediate(smc.combined);
                }
            }

            if (!smc.savedPrefab) {
                if (GUILayout.Button("Save Mesh")) {
                    string n = smc.MeshName;
                    if (!System.IO.Directory.Exists(savedMeshLoc)) {
                        System.IO.Directory.CreateDirectory(savedMeshLoc);
                    }
                    if (System.IO.Directory.Exists(savedMeshLoc)) {
                        if (!System.IO.File.Exists(savedMeshLoc +
                                                   smc.meshName +
                                                   ".asset")) {
                            Mesh mesh = smc.combined.GetComponent<MeshFilter>().sharedMesh;
                            AssetDatabase.CreateAsset(mesh, savedMeshLoc + n + ".asset");
                            smc.advanced = false;
                            smc.savedPrefab = true;
                            Debug.Log(savedMeshLoc+n+".asset");
                        } else {
                            Debug.Log(smc.MeshName + ".asset" + " already exists, "+
                                  "please change the name");
                        }
                    }
                }
                smc.MeshName = GUILayout.TextField(smc.meshName);
            }

        }

        if (GUI.changed) {
            EditorUtility.SetDirty(smc);
        }
    }

    void Toggle(string label, ref bool option) {
        option = EditorGUILayout.Toggle(label, option);
    }

    void Info(string info) {
        GUILayout.Space(10);
        GUILayout.Label(info);
        GUILayout.Space(10);
    }

    void CombineMeshes() {
        MeshCombinerCore.Combine(smc);
    }

}
#endif
