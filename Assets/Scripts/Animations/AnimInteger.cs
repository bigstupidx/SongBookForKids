using UnityEngine;

namespace Syng {

    public class AnimInteger : MonoBehaviour {

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private string   integerName;

        public void Trigger(int i) {
            if (animator == null) {
                return;
            }
            animator.SetInteger(integerName, i);
        }

        public void SetAnimator(Animator animator) {
            this.animator = animator;
        }
    }
}
