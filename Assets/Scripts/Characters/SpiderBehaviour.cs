using UnityEngine;
using UnityEngine.EventSystems;

namespace Syng {
    [RequireComponent(typeof(AudioSource))]
    public class SpiderBehaviour : MonoBehaviour, IPointerClickHandler {

        public int          songIndex;
        public Animator     animatorSpider;

        private MPlaySong   playSong;

        void Awake() {
            playSong = new MPlaySong {
                songSource = GetComponent<AudioSource>()
            };
        }

        public void OnPointerClick(PointerEventData data) {
            playSong.songIndex = songIndex;
            Main.msg.Dispatch(playSong);
            if (playSong.canPlay) {
                animatorSpider.SetTrigger("Sing");
            }
        }
    }
}
