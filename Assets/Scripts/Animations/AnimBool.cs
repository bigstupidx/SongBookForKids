using UnityEngine;

namespace Syng {
    public class AnimBool : MonoBehaviour {

        public Animator animator;
        public string   boolName;

        public void Trigger(bool value) {
            animator.SetBool(boolName, value);
        }
    }
}
