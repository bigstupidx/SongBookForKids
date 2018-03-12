using UnityEngine;

namespace Syng {
    [RequireComponent(typeof(Flipbook))]
    public class SkippingStone : MonoBehaviour {

        private Flipbook    flipbook;

        void OnEnable() {
            flipbook = GetComponent<Flipbook>();
            // Just in case Flipbook is set to autoplay
            flipbook.Stop();
        }

        public void Play() {
            flipbook.FreezeFrame(Random.Range(0, flipbook.numFrames - 1));
        }

    }
}
