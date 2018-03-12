using UnityEngine;

namespace Syng {

    [AddComponentMenu("Syng/Position Bounds")]
    public class PositionBounds : MonoBehaviour {

        [SerializeField]
        private Bounds bounds;

        public Vector3 ClampPosition(Vector3 pos) {
            Vector3 ext = bounds.extents;
            return new Vector3(Clamp(pos.x, ext.x),
                               Clamp(pos.y, ext.y),
                               Clamp(pos.z, ext.z));
        }

        public Vector3 ClampX(Vector3 pos) {
            return new Vector3(Clamp(pos.x, bounds.extents.x), pos.y, pos.z);
        }

        private float Clamp(float val, float ext) {
            return Mathf.Clamp(val, -ext, ext);
        }

        public float width {
            get { return bounds.size.x; }
        }

        public float height {
            get { return bounds.size.y; }
        }

        public float depth {
            get { return bounds.size.z; }
        }
    }
}
