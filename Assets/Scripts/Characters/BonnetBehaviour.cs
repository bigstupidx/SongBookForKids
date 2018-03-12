using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Syng {
    [RequireComponent(typeof(Animator))]
    public class BonnetBehaviour : MonoBehaviour, IPointerClickHandler {

        public UnityEvent   onChange;

        private Animator    animator;
        private int         curPosition;

        void Awake() {
            curPosition = 0;
            animator = GetComponent<Animator>();
        }

        public void OnPointerClick(PointerEventData data) {
            curPosition = (curPosition + 1) % 2;
            animator.SetInteger("Position", curPosition);
            onChange.Invoke();
        }
    }
}
