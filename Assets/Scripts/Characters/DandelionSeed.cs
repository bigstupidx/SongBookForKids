using UnityEngine;
using UnityEngine.EventSystems;

namespace Syng {
    public class DandelionSeed : MonoBehaviour, IPointerClickHandler {

        public float                rotationRange = 15f;
        public float                rotationSpeed = 0.5f;

        private float               speed = 1f;
        private float               pos = 0f;
        private bool                isDrifting;
        private BezierCurve         bezierCurve;
        private DandelionBehaviour  dandelion;
        private Vector3             origin;
        private Quaternion          rot;
        private float               r;

        void Awake() {
            rot = transform.rotation;
            origin = transform.localPosition;
            isDrifting = false;
        }

        void Update() {
            if (!isDrifting) {
                return;
            }

            Vector3 point = bezierCurve.GetPointAt(pos);
            point = bezierCurve.transform.InverseTransformPoint(point);
            transform.localPosition = origin + point;

            float rad = Mathf.Sin(r);
            r += Time.deltaTime / rotationSpeed;
            r %= Mathf.PI * 2f;
            Vector3 sway = Vector3.forward * rotationRange * rad;
            transform.rotation = Quaternion.Euler(sway + rot.eulerAngles);

            pos += Time.deltaTime / speed;
            if (pos >= 0.99f) {
                pos = 0f;
                isDrifting = false;
                transform.localPosition = origin;
                transform.rotation = rot;
                dandelion.OnSeedPopped();
            }
        }

        public void OnPointerClick(PointerEventData data) {
            if (!isDrifting) {
                return;
            }
            dandelion.OnSeedPopped();
            isDrifting = false;
            r = 0f;
        }

        public void SetOwner(DandelionBehaviour dandelion) {
            this.dandelion = dandelion;
        }

        public void Drift(BezierCurve   curve,
                          float         speed) {
            bezierCurve = curve;
            this.speed = speed;
            isDrifting = true;
        }

        public bool IsDrifting() {
            return isDrifting;
        }
    }
}
