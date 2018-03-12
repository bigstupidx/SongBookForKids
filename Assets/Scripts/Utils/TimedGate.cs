using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Syng {
    public class TimedGate : MonoBehaviour {

        [SerializeField]
        private float       duration;
        [SerializeField]
        private UnityEvent  onPassThrough;
        private bool        canPassThrough;

        void Awake() {
            canPassThrough = true;
        }

        IEnumerator Countdown() {
            canPassThrough = false;
            yield return new WaitForSeconds(duration);
            canPassThrough = true;
        }

        public void PassThrough() {
            if (canPassThrough) {
                onPassThrough.Invoke();
                StartCoroutine(Countdown());
            }
        }
    }
}
