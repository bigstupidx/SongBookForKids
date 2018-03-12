using UnityEngine;
using System.Collections;

namespace Syng {
    public class StarsBehaviour : MonoBehaviour {

        [SerializeField]
        [Min(1)]
        private int             defaultConcurrentBlinks;
        [SerializeField]
        private StarBehaviour[] stars;
        private int[]           sequence;
        private IEnumerator     blinking;

        void Awake() {
            sequence = new int[stars.Length];
            for (int i=0; i<sequence.Length; i++) {
                sequence[i] = i;
            }
        }

        public void Stop() {
            if (blinking == null) {
                return;
            }
            StopCoroutine(blinking);
        }

        public void Blink(float interval, int repeats, int concurrent) {
            blinking = BlinkStars(interval, repeats, concurrent);
            StartCoroutine(blinking);
        }

        public void Burst() {
            Randomize(sequence);
            blinking = BlinkStars(0.25f,
                                  stars.Length,
                                  defaultConcurrentBlinks);
            StartCoroutine(blinking);
        }

        public void DisableTap() {
            for (int i=0; i<stars.Length; i++) {
                stars[i].enabled = false;
            }
        }

        public void EnableTap() {
            for (int i=0; i<stars.Length; i++) {
                stars[i].enabled = true;
            }
        }

        IEnumerator BlinkStars(float interval, int repeats, int concurrent) {
            int count = 0;
            int curStar = 0;
            while (count < repeats) {
                for (int i=0; i<concurrent; i++) {
                    StarBehaviour star = stars[sequence[curStar]];
                    star.Blink();
                    curStar++;
                    if (curStar >= stars.Length) {
                        Randomize(sequence);
                        curStar = 0;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(interval);
                count++;
            }
        }

        public void Randomize(int[] sequence) {
            // Randomize using Fisher-Yates algorithm
            for (int i=sequence.Length; i>1; i--) {
                int j = Random.Range(0, (i-1));
                int tmp = sequence[j];
                sequence[j] = sequence[i - 1];
                sequence[i - 1] = tmp;
            }
        }

    }
}
