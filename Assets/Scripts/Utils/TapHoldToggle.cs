using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Syng {
    public class TapHoldToggle : MonoBehaviour {

        [SerializeField]
        private float       minHoldTime;
        [SerializeField]
        private UnityEvent  toggleOn;
        [SerializeField]
        private UnityEvent  toggleOff;
        private bool        toggledOn;
        private bool        toggledOff;
        private float       holdTime;
        private float       endHoldTime;

        void Awake() {
            holdTime = float.PositiveInfinity;
        }

        public void OnHold(PointerEventData data) {
            // holdTime is the minimum time to hold.
            // toggle off when Time.time is greater than holdTime
            holdTime = Time.time + 0.45f;
            if (!toggledOn) {
                endHoldTime = Time.time + minHoldTime;
                toggleOn.Invoke();
                toggledOn = true;
                toggledOff = false;
            }
        }

        void Update() {
            if (Time.time > holdTime && Time.time > endHoldTime) {
                if (!toggledOff) {
                    toggleOff.Invoke();
                    toggledOn = false;
                    toggledOff = true;
                }
            }
        }
    }
}
