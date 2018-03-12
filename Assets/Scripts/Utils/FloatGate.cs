using UnityEngine;
using UnityEngine.Events;

namespace Syng {
    public class FloatGate : MonoBehaviour {

        [SerializeField]
        private float       threshold;
        [SerializeField]
        private UnityEvent  onAboveThreshold;
        [SerializeField]
        private UnityEvent  onBelowThreshold;
        private bool        isAbove;
        private bool        isBelow;

        void Awake() {
            isAbove = false;
            isBelow = false;
        }

        public void Validate(float f) {
            if (f >= threshold && !isAbove) {
                onAboveThreshold.Invoke();
                isAbove = true;
                isBelow = false;
            }
            if (f < threshold && !isBelow) {
                onBelowThreshold.Invoke();
                isBelow = true;
                isAbove = false;
            }
        }
    }
}
