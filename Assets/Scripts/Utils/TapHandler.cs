using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Syng {

    // Should be renamed to something more general. But will break
    // all instances in Main scene if I do.
    [System.Serializable]
    public class PointerClickEvent : UnityEvent<PointerEventData> {}

    public class TapHandler : MonoBehaviour,
                              IPointerHoldHandler,
                              IPointerClickHandler,
                              IPointerDownHandler,
                              IPointerUpHandler,
                              IPointerExitHandler {

        public PointerClickEvent    onPointerHold;
        public PointerClickEvent    onPointerClick;
        public PointerClickEvent    onPointerDown;
        public PointerClickEvent    onPointerPress;
        public PointerClickEvent    onPointerRelease;

        private bool                down;
        private PointerEventData    data;

        void Awake() {
            down = false;
        }

        void Update() {
            if (!down) {
                return;
            }
            onPointerPress.Invoke(data);
        }

        public void OnPointerHold(PointerEventData data) {
            onPointerHold.Invoke(data);
        }

        public void OnPointerClick(PointerEventData data) {
            onPointerClick.Invoke(data);
        }

        public void OnPointerDown(PointerEventData data) {
            onPointerDown.Invoke(data);
            this.data = data;
            down = true;
        }

        public void OnPointerUp(PointerEventData data) {
            down = false;
            onPointerRelease.Invoke(data);
        }

        public void OnPointerExit(PointerEventData data) {
            down = false;
            onPointerRelease.Invoke(data);
        }

    }
}
