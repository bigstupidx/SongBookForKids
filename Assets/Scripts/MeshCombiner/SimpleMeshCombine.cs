using UnityEngine;

[AddComponentMenu("Hyper/Simple Mesh Combine")]
public class SimpleMeshCombine : MonoBehaviour {

    // Stores gameObjects that has been merged, mesh renderer disabled
    [SerializeField]
    public GameObject[] combinedGameObjects;

    // Stores the combined mesh gameObject
    [SerializeField]
    public GameObject combined;

    // Asset name when saving as prefab
    [SerializeField]
    public string meshName = "Combined_Meshes";

    // Toggles advanced features
    [SerializeField]
    public bool advanced;

    // Used when checking if this mesh has been saved to prefab
    // (saving the same mesh twice generates error)
    [SerializeField]
    public bool savedPrefab = false;

    // Toggles secondary UV map generation
    [SerializeField]
    public bool generateLightmapUV;

    public string MeshName {
        get { return meshName; }
        set { meshName = value; }
    }
}
