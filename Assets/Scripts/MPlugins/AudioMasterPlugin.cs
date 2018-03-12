using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Mandarin;
using Mandarin.PluginSystem;

namespace Syng {

    public class MStopSong : IMessage {}

    public class MPlaySong : IMessage {
        public AudioSource  songSource;
        public int          songIndex;
        public string       songName;
        public bool         otherSongPlaying;
        public bool         canPlay = false;
    }

    public class MSongDone : IMessage {
        public int          songIndex;
    }

    [Serializable]
    public class AudioChannel {
        public string           name;
        public AnimationCurve   curve;
        public float            fadeInDuration;
        public float            fadeOutDuration;
        [Range(-80f, 0f)]
        public float            maxVolume;
        [Range(-80f, 0f)]
        public float            minVolume;
    }

    [RegisterPlugin]
    public class AudioMasterPlugin : MonoBehaviour, IPlugin {

        [SerializeField]
        private AudioMixer      mixer;
        private List<AudioClip> songs;
        [SerializeField]
        private AudioChannel    chAmbient;
        [SerializeField]
        private AudioChannel    chMusic;

        private int             curSong;
        private AudioSource     source;
        private MSongDone       mSongDone;
        private Messenger       msg;

		private SetupAudioSession mSetupAudioSession;

        public void Init(Messenger msg) {
            this.msg = msg;
            curSong = -1;
            mSongDone = new MSongDone();
            songs = new List<AudioClip>(16);
            msg.AddListener<MPlaySong>(OnPlaySong);
            msg.AddListener<MStopSong>(OnStopSong);
			mSetupAudioSession = new SetupAudioSession();
			mSetupAudioSession.Start();
        }

        public void Ready(Messenger msg) {
        }

        public void OnLoadedAudio(AudioClip clip, int i) {
            if (songs.Count > i) {
                songs[i] = clip;
            } else {
                songs.Insert(i, clip);
            }
        }

        public void OnUnloadAudio(AudioClip clip, int i) {
            if (i == curSong) {
                source.Stop();
                source.clip = null;
                curSong = -1;
            }
            songs[i] = null;
        }

        private void OnStopSong(MStopSong stopSong) {
            if (curSong < 0) {
                return;
            }

            FadeIn(chAmbient);
            FadeOut(chMusic);

            mSongDone.songIndex = curSong;
            msg.Dispatch(mSongDone);
            curSong = -1;
            source.Stop();
            source.clip = null;
        }

        private void OnPlaySong(MPlaySong playSong) {
            if (playSong.songIndex < 0) {
                playSong.canPlay = false;
                playSong.otherSongPlaying = false;
                return;
            }

            if (playSong.songIndex == curSong ||
                curSong >= 0) {
                playSong.canPlay = false;
                playSong.otherSongPlaying = playSong.songIndex != curSong;
                return;
            }

            FadeIn(chMusic);
            FadeOut(chAmbient);

            curSong = playSong.songIndex;
            source = playSong.songSource;
            source.clip = songs[curSong];
            source.Play();
            playSong.canPlay = true;
            playSong.songName = source.clip.name;
            playSong.otherSongPlaying = false;
        }

        void Start() {
            mixer.SetFloat("ambVol", -80f);
            mixer.SetFloat("musicVol", -80f);

            FadeIn(chAmbient);
        }

        void Update() {
            if (curSong < 0) {
                return;
            }

            // cross fade song -> amb
            float remaining = source.clip.length - source.time;
            int progFadeOut = (int)(Mathf.Clamp01(remaining / chMusic.fadeOutDuration) * 100);

            float progress = progFadeOut / 100f;
            float fadeMusic = chMusic.curve.Evaluate(progress); // 1 -> 0
            float fadeAmb = chAmbient.curve.Evaluate(progress); // 1 -> 0

            SetVolume(chMusic,   fadeMusic,     chMusic.minVolume,   chMusic.maxVolume);
            SetVolume(chAmbient, 1 - fadeAmb,   chAmbient.minVolume, chAmbient.maxVolume);

            if (remaining <= 0.1f) {
                mSongDone.songIndex = curSong;
                msg.Dispatch(mSongDone);
                curSong = -1;
            }
        }

        private void SetVolume(string channel, float value) {
            mixer.SetFloat(channel, value);
        }

        private void SetVolume(AudioChannel     channel,
                               float            progress,
                               float            volStart,
                               float            volEnd) {
            float fade = channel.curve.Evaluate(progress);
            float vol = Map(fade, 0f, 1f, volStart, volEnd);
            mixer.SetFloat(channel.name, vol);
        }

        private float Map(float val, float mina, float maxa, float minb, float maxb) {
            return minb + (maxb - minb) * ((val - mina) / (maxa - mina));
        }

        private void FadeOut(AudioChannel channel) {
            float startVol = -80f;
            mixer.GetFloat(channel.name, out startVol);
            StartCoroutine(FadeChannel(channel,
                                       startVol,
                                       channel.maxVolume,
                                       channel.minVolume,
                                       channel.fadeOutDuration));
        }

        private void FadeIn(AudioChannel channel) {
            float startVol = -80f;
            mixer.GetFloat(channel.name, out startVol);
            StartCoroutine(FadeChannel(channel,
                                       startVol,
                                       channel.minVolume,
                                       channel.maxVolume,
                                       channel.fadeInDuration));
        }

        IEnumerator FadeChannel(AudioChannel    channel,
                                float           curVolume,
                                float           volStart,
                                float           volEnd,
                                float           duration) {
            float p = Map(curVolume, volStart, volEnd, 0f, 1f);
            float time = Mathf.Clamp01(p) * duration;

            while (time <= duration) {
                float progress = time / duration;
                SetVolume(channel, progress, volStart, volEnd);
                time += Time.deltaTime;
                yield return new WaitForSeconds(0);
            }

            SetVolume(channel, 1f, volStart, volEnd);
        }
    }
}
