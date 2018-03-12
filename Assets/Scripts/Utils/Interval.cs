using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Syng {
    public class Interval : MonoBehaviour {

        public float        delay;
        public float        intervalMin;
        public float        intervalMax;
        public UnityEvent   onTick;
        public UnityEvent   onForceTick;

        private bool        doTick;

        void Awake() {
            doTick = true;
        }

        void Start() {
            StartCoroutine(Wait(delay));
        }

        public void ForceTick() {
            if (doTick) {
                StartCoroutine(LockTick(intervalMax));
            }
            doTick = false;
            onForceTick.Invoke();
        }

        IEnumerator LockTick(float delay) {
            yield return new WaitForSeconds(delay);
            doTick = true;
        }

        IEnumerator Wait(float delay) {
            yield return new WaitForSeconds(delay);
            NextTick();
        }

        IEnumerator Tick() {
            float interval = (intervalMax - intervalMin) * Random.value + intervalMin;
            yield return new WaitForSeconds(interval);
            NextTick();
        }

        private void NextTick() {
            if (doTick) {
                onTick.Invoke();
            }
            StartCoroutine(Tick());
        }

    }
}
