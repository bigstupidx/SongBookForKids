using UnityEngine;

namespace Syng {
    [RequireComponent(typeof(PlaySong))]
    public class MoonBehaviour : MonoBehaviour {

        [SerializeField]
        private StarsBehaviour  stars;
        [SerializeField]
        private float           blinkInterval;
        [SerializeField]
        private int             blinkRepetitions;
        [SerializeField]
        [Min(1)]
        private int             concurrentBlinks;

        private PlaySong        playSong;

        void Awake() {
            playSong = GetComponent<PlaySong>();
        }

        void Start() {
            Main.msg
                .AddListener<MPlaySong>(OnPlaySong)
                .AddListener<MSongDone>(OnSongDone);
        }

        public void SetStars(StarsBehaviour stars) {
            this.stars = stars;
        }

        private void OnSongDone(MSongDone msg) {
            if (msg.songIndex == playSong.song) {
                stars.EnableTap();
            }
        }

        private void OnPlaySong(MPlaySong msg) {
            if (msg.songIndex == playSong.song &&
                !msg.otherSongPlaying) {
                stars.DisableTap();
            }
        }

        public void OnSing() {
            stars.Blink(blinkInterval, blinkRepetitions, concurrentBlinks);
        }

        public void OnCannotSing(bool otherSongPlaying) {
            if (otherSongPlaying) {
                stars.Burst();
            }
        }
    }
}
