using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CustomBounds : MonoBehaviour {

    [SerializeField]
    private Vector3 size;
    [SerializeField]
    private Vector3 center;

    void Awake() {
        MeshFilter mf = GetComponent<MeshFilter>();
        Bounds bounds = new Bounds(center, size);
        mf.sharedMesh.bounds = bounds;
    }
}
