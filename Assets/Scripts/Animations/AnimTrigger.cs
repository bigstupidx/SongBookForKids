using UnityEngine;

namespace Syng {
    public class AnimTrigger : MonoBehaviour {

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private string   triggerName;

        public void Trigger() {
            if (animator == null) {
                return;
            }
            animator.SetTrigger(triggerName);
        }

        public void SetAnimator(Animator animator) {
            this.animator = animator;
        }
    }
}
