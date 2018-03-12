using UnityEngine;

namespace Syng {
    public class GetAnimator : MonoBehaviour {

        [SerializeField]
        private AnimatorEvent onGotAnimator;

        public void OnGotGameObject(GameObject go, int i) {
            OnGotGameObject(go);
        }

        public void OnGotGameObject(GameObject go) {
            Animator a = go.GetComponent<Animator>();
            if (a == null) {
                return;
            }
            onGotAnimator.Invoke(a);
        }

    }
}
