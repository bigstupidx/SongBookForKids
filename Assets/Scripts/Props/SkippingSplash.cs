using UnityEngine;
using UnityEngine.Events;

namespace Syng {

    public class SplashAnimEvent : UnityEvent<SkippingSplash> {}

    [RequireComponent(typeof(Flipbook))]
    public class SkippingSplash : MonoBehaviour {

        private Flipbook        flipbook;
        public SplashAnimEvent  onAnimDone = new SplashAnimEvent();

        void Awake() {
            flipbook = GetComponent<Flipbook>();
            flipbook.Stop();
        }

        public void Play() {
            flipbook.Play();
        }

        public void OnAnimDone() {
            onAnimDone.Invoke(this);
        }
    }
}
