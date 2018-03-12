using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Syng {
    public class TailLights : MonoBehaviour {

        public MeshRenderer glowTop;
        public MeshRenderer glowBottom;
        public float        blinkDuration;
        public int          repetitions;

        public UnityEvent   onBlink;

        private float       blinkDurationOrig;
        private int         repetitionsOrig;
        private bool        isBlinking;

        void Awake() {
            blinkDurationOrig = blinkDuration;
            repetitionsOrig = repetitions;
            isBlinking = false;
            glowTop.enabled = false;
            glowBottom.enabled = false;
        }

        public void BlinkLights() {
            if (isBlinking) {
                repetitions += 4;
                blinkDuration -= 0.1f;
                blinkDuration = Mathf.Clamp(blinkDuration, 0.1f, blinkDurationOrig);
                return;
            }
            isBlinking = true;
            StartCoroutine(Blink());
        }

        IEnumerator Blink() {
            glowTop.enabled = true;
            glowBottom.enabled = false;
            int count = 0;

            while (count < repetitions) {
                onBlink.Invoke();
                yield return new WaitForSeconds(blinkDuration);
                glowTop.enabled = !glowTop.enabled;
                glowBottom.enabled = !glowBottom.enabled;
                count++;
            }

            glowTop.enabled = false;
            glowBottom.enabled = false;
            blinkDuration = blinkDurationOrig;
            repetitions = repetitionsOrig;
            isBlinking = false;
        }
    }
}
