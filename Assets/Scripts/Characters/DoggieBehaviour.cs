using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace Syng {
    [RequireComponent(typeof(AudioSource))]
    public class DoggieBehaviour : MonoBehaviour {

        public int          songIndex;
        public Animator     animatorSing;
        public Animator     animatorDrift;

        public UnityEvent   onRow;

        private MPlaySong   playSong;
        private bool        ready;

        void Awake() {
            ready = true;
            playSong = new MPlaySong {
                songSource = GetComponent<AudioSource>()
            };
        }

        public void OnSing(PointerEventData data) {
            if (!ready) {
                return;
            }
            playSong.songIndex = songIndex;
            Main.msg.Dispatch(playSong);
            if (playSong.canPlay) {
                ready = false;
                onRow.Invoke();
                animatorSing.SetTrigger("Sing");
                animatorDrift.SetTrigger("Drift");
                StartCoroutine(Countdown());
            }
        }

        IEnumerator Countdown() {
            yield return new WaitForSeconds(26.1f);
            ready = true;
        }
    }
}
