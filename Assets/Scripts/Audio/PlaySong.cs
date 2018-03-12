using UnityEngine;
using UnityEngine.Events;

namespace Syng {

    [RequireComponent(typeof(AudioSource))]
    public class PlaySong : MonoBehaviour {

        [SerializeField]
        private int             songIndex;
        [SerializeField]
        private UnityEvent      onCanPlay;
        [SerializeField]
        private BoolEvent       onCannotPlay;

        private MPlaySong       playSong;
        private MStopSong       stopSong;
        private MAnalyticSong   logSong;

        void Awake() {
            stopSong = new MStopSong();
            playSong = new MPlaySong {
                songSource = GetComponent<AudioSource>()
            };
            logSong = new MAnalyticSong();
        }

        public void Play() {
            playSong.songIndex = songIndex;
            Main.msg.Dispatch(playSong);
            if (playSong.canPlay) {
                onCanPlay.Invoke();
                logSong.songName = playSong.songName;
                Main.msg.Dispatch(logSong);
            } else {
                onCannotPlay.Invoke(playSong.otherSongPlaying);
            }
        }

        public void Stop() {
            Main.msg.Dispatch(stopSong);
        }

        public int song {
            get { return songIndex; }
        }
    }
}
