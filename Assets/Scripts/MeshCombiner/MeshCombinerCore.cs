#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class MeshCombinerCore {

    public static void Combine(SimpleMeshCombine target) {
        MeshFilter[] meshFilters = FindEnabledMeshes(target.transform);

        if (meshFilters.Length > 0) {
            GameObject combinedFrags = new GameObject();
            combinedFrags.AddComponent<MeshFilter>();
            combinedFrags.AddComponent<MeshRenderer>();
            MeshInstance[] instances = new MeshInstance[meshFilters.Length];
            GameObject[] combinedGOs = new GameObject[meshFilters.Length];
            MeshFilter matFilter = null;

            for (int i=0; i<meshFilters.Length; i++) {
                MeshFilter mf = meshFilters[i];
                if (i == meshFilters.Length-1) {
                    matFilter = mf;
                }
                combinedGOs[i] = mf.gameObject;
                MeshInstance mi = new MeshInstance();
                mi.mesh = mf.sharedMesh;
                // D.Log(mi.mesh.vertices.Length);
                // mi.subMeshIndex = mf.sharedMesh.subMeshCount;
                mi.subMeshIndex = 0;
                mi.transform = mf.transform.localToWorldMatrix;
                instances[i] = mi;
            }

            target.combinedGameObjects = combinedGOs;
            Mesh m = MeshCombineUtility.Combine(instances, false);

            // D.Log(target.transform.name+
            //       " Combined " + meshFilters.Length + " Meshes");
            // D.Warn("Mesh: " + m.vertices.Length);

            // CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            // GameObject[] mfGOs = new GameObject[meshFilters.Length];
            // for (int i=0; i<meshFilters.Length; i++) {
            //     MeshFilter mf = meshFilters[i];
            //     MeshRenderer mr = combinedFrags.GetComponent<MeshRenderer>();
            //     mr.sharedMaterial = mf.transform.GetComponent<MeshRenderer>().sharedMaterial;
            //     mfGOs[i] = meshFilters[i].gameObject;
            //     CombineInstance ci = combine[i];
            //     ci.mesh = mf.transform.GetComponent<MeshFilter>().sharedMesh;
            //     ci.transform = mf.transform.localToWorldMatrix;
            //     combine[i] = ci;
            // }

            combinedFrags.GetComponent<MeshFilter>().mesh = m;
            combinedFrags.GetComponent<MeshRenderer>().sharedMaterial = matFilter.GetComponent<Renderer>().sharedMaterial;
            // combinedFrags.GetComponent<MeshFilter>().mesh = new Mesh();
            // combinedFrags.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

            // // Disabled for now
            // // if (target._generateLightmapUV){
            // //     Unwrapping.GenerateSecondaryUVSet(combinedFrags.GetComponent(MeshFilter).sharedMesh);
            // //     combinedFrags.isStatic = true;
            // // }

            combinedFrags.name = "_Combined Mesh [" + target.transform.name + "]";
            target.combined = combinedFrags.gameObject;
            combinedFrags.transform.parent = target.transform;
            EnableRenderers(meshFilters, false);
        }
    }

    public static void EnableRenderers(MeshFilter[] filters, bool e) {
        for (int i=0; i<filters.Length; i++){
            filters[i].GetComponent<Renderer>().enabled = e;
        }
    }

    public static void EnableRenderers(GameObject[] gos, bool e) {
        for (int i=0; i<gos.Length; i++){
            gos[i].GetComponent<Renderer>().enabled = e;
        }
    }

    // Returns a meshFilter[] list of all renderer enabled
    // meshfilters(so that it does not merge disabled meshes,
    // useful when there are invisible box colliders)
    public static MeshFilter[] FindEnabledMeshes(Transform target) {
        Component[] renderers;
        int count = 0;
        renderers = target.GetComponentsInChildren<MeshFilter>();
        // D.Log(renderers.Length);

        for (int i=0; i<renderers.Length; i++) {
            Component comp = renderers[i];
            MeshRenderer mr = comp.GetComponent<MeshRenderer>();
            if (mr != null && mr.enabled) {
                // D.Log(mr.gameObject.name);
                count++;
            }
        }

        //creates a new array with the correct length
        MeshFilter[] meshfilters = new MeshFilter[count];
        count = 0;
        for (int j=0; j<renderers.Length; j++) {
            Component comp = renderers[j];
            MeshRenderer mr = comp.GetComponent<MeshRenderer>();
            if (mr != null && mr.enabled) {
                meshfilters[count] = (MeshFilter)renderers[j];
                count++;
            }
        }

        return meshfilters;
    }

}
#endif