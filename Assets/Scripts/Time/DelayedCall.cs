using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Syng {
    public class DelayedCall : MonoBehaviour {

        [SerializeField]
        private float delay;

        [SerializeField]
        private UnityEvent onCallMethod;

        private bool ready;

        void Awake() {
            ready = true;
        }

        public void Call() {
            if (!ready) {
                return;
            }
            StartCoroutine(OnCall(delay));
        }

        private IEnumerator OnCall(float sec) {
            ready = false;
            yield return new WaitForSeconds(sec);
            onCallMethod.Invoke();
            ready = true;
        }
    }
}
