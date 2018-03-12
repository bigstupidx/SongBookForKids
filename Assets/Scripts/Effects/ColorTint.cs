using UnityEngine;

namespace Syng {
    [RequireComponent(typeof(MeshFilter))]
    public class ColorTint : MonoBehaviour {

        [SerializeField]
        private Color color;
        [SerializeField]
        private Mesh originalMesh;

        void Awake() {
            MeshFilter mf = GetComponent<MeshFilter>();
            Mesh original = mf.sharedMesh ?? originalMesh;
            Mesh mesh = Mesh.Instantiate(original);
            mf.mesh = SetColors(mesh, original.name + "_Colored");
        }

        public void ForceUpdate() {
            if (originalMesh == null) {
                D.Warn("Missing original mesh for", gameObject.name);
            }
            GetComponent<MeshFilter>().mesh = SetColors(Mesh.Instantiate(originalMesh),
                                                        originalMesh.name + "_Colored");
        }

        public Mesh SetColors(Mesh mesh, string mname) {
            mesh.name = mname;
            Vector3[] vertices = mesh.vertices;
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                colors[i] = color;
            }
            mesh.colors = colors;
            return mesh;
        }

        public Mesh GetOriginalMesh() {
            return originalMesh;
        }

        public void SetOriginalMesh(Mesh mesh) {
            originalMesh = mesh;
        }

        public void SetColor(Color color) {
            this.color = color;
        }
    }
}
