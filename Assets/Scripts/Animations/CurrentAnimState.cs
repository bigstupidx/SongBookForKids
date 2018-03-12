using UnityEngine;
using UnityEngine.Events;

namespace Syng {
    public class CurrentAnimState : MonoBehaviour {

        [SerializeField]
        private Animator   animator;
        [SerializeField]
        private string     animState;
        [SerializeField]
        private int        layer;
        [SerializeField]
        private UnityEvent onStateValid;
        [SerializeField]
        private UnityEvent onStateInvalid;

        private int        stateHash;

        void Awake() {
            stateHash = Animator.StringToHash(animState);
        }

        public void ValidateCurrentState() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(layer);
            if (state.fullPathHash == stateHash) {
                onStateValid.Invoke();
            } else {
                onStateInvalid.Invoke();
            }
        }
    }
}
