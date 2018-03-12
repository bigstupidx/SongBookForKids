using UnityEngine;

namespace Syng {
    [RequireComponent(typeof(PlaySound))]
    public class BirdieBehaviour : MonoBehaviour {

        public Animator         animatorLarge;
        public Animator         animatorSmall;

        private PlaySound       playSound;
        private AudioSource[]   audioSources;

        void Awake() {
            playSound = GetComponent<PlaySound>();
            audioSources = new AudioSource[2];
        }

        public void TapLargeBird() {
            if (Play(audioSources, 0)) {
                animatorLarge.SetTrigger("Sing");
            }
        }

        public void TapSmallBird() {
            if (Play(audioSources, 1)) {
                animatorSmall.SetTrigger("Sing");
            }
        }

        private bool Play(AudioSource[] sources, int n) {
            if (sources[n] != null && sources[n].isPlaying) {
                return false;
            }

            sources[n] = playSound.GetNextAudioSource();
            if (sources[n] == null) {
                return false;
            }

            playSound.Play(sources[n], playSound.GetNextClip());
            return true;
        }
    }
}
