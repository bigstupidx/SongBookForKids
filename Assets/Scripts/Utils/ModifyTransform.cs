using UnityEngine;
using System;

namespace Syng {
    public class ModifyTransform : MonoBehaviour {

        public AnimationCurve lerpCurve;
        public float lerpDuration;

        private float lerpTargetTime;
        private Vector3 targetPos;
        private Vector3 startPos;

        private Action<Vector3>[] posUpdates;
        private int posUpdate;

        void Awake() {
            posUpdates = new Action<Vector3>[] {
                SetLocalPosition, AddToLocalPosition,
                SetWorldPosition, AddToWorldPosition
            };
        }

        void Update() {
            if (lerpTargetTime > 0f) {
                Vector3 lerpDir = (targetPos - startPos).normalized
                    * lerpCurve.Evaluate((lerpDuration - lerpTargetTime) / lerpDuration);
                posUpdates[posUpdate](lerpDir);
                lerpTargetTime -= Time.deltaTime;
            }
        }

        public void LerpLocalPosition(Vector3 pos) {
            startPos = transform.localPosition;
            targetPos = pos;
            lerpTargetTime = lerpDuration;
            posUpdate = 1;
        }

        public void LerpWorldPosition(Vector3 pos) {
            startPos = transform.position;
            targetPos = pos;
            lerpTargetTime = lerpDuration;
            posUpdate = 3;
        }

        public void AddToLocalPosition(Vector3 pos) {
            transform.localPosition += pos;
        }

        public void SetLocalPosition(Vector3 pos) {
            transform.localPosition = pos;
        }

        public void AddToWorldPosition(Vector3 pos) {
            transform.position += pos;
        }

        public void SetWorldPosition(Vector3 pos) {
            transform.position = pos;
        }

        // public void SetRotation(Quaternion rot) {}
        // public void SetScale(Vector3 scale) {}
    }
}
