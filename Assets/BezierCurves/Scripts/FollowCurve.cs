using UnityEngine;

namespace Syng {
    public class FollowCurve : MonoBehaviour {

        public BezierCurve      bezierCurve;
        public float            speed = 1;

        private float           pos = 0f;

        void Update() {
            Vector3 point = bezierCurve.GetPointAt(pos);
            transform.localPosition = point;
            pos += Time.deltaTime / 3f;
            if (pos > 1f) {
                pos = 0f;
            }
        }
    }
}
