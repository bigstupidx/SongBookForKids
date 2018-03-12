using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Syng {
    public class StarBehaviour : MonoBehaviour, IPointerClickHandler {

        public Animator     animator;
        public UnityEvent   onClick;

        public void Blink() {
            animator.SetTrigger("Twinkle");
        }

        public void OnPointerClick(PointerEventData data) {
            onClick.Invoke();
            Blink();
        }
    }
}
