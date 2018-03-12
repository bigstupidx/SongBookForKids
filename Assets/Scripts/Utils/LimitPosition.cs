using UnityEngine;

namespace Syng {

    public enum LimitPositionMode {
        RECTANGLE = 0,
        CIRCLE = 1,
        OVAL = 2
    }

    public class LimitPosition : MonoBehaviour {

        public LimitPositionMode mode;

        [SerializeField]
        private float radiusH;
        [SerializeField]
        private float radiusV;

        void LateUpdate() {
            transform.localPosition = Clamp(transform.localPosition);
        }

        private Vector3 Clamp(Vector3 pos) {
            float dot = Mathf.Clamp(Vector3.Dot(Vector3.up, pos), -1, 1);
            dot = dot > 0f ? 1f : dot < 0f ? -1f : 0f;
            float angles = Vector3.Angle(pos, Vector3.right) * Mathf.Deg2Rad * dot;
            Vector3 clamped = new Vector3(Mathf.Cos(angles) * radiusH,
                                          Mathf.Sin(angles) * radiusV,
                                          0f);
            return pos.magnitude < clamped.magnitude ? pos : clamped;
        }
    }
}
