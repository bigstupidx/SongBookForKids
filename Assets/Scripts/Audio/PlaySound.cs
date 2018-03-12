using UnityEngine;
using UnityEngine.Audio;
using Mandarin;

namespace Syng {

    public class PlaySound : MonoBehaviour {

        [SerializeField]
        private AudioClip[]     audioClips;
        [SerializeField]
        private PlayOrder       playOrder;
        [SerializeField]
        [Min(1)]
        private int             numAudioSources;
        [SerializeField]
        private AudioMixerGroup output;
        [SerializeField]
        [Range(0f, 1f)]
        private float           volume;
        [SerializeField]
        private float           maxDistance;
        [SerializeField]
        private int[]           sequence;
        private AudioSource[]   audioSources;
        private IPlayOrder[]    orderers;
        private int             curClip;

        void Awake() {
            Main.msg.AddListener<MCameraReady>(OnCameraReady);

            curClip = 0;

            if ((int)playOrder != 2) {
                sequence = new int[audioClips.Length];
                // Fill the array, otherwise the randomization won't have anything
                // to randomize
                for (int i=0; i<sequence.Length; i++) {
                    sequence[i] = i;
                }
            }

            orderers = new IPlayOrder[] {
                new PlayOrderSeq(),
                new PlayOrderRnd(),
                new PlayOrderBlank(),
                new PlayOrderSeq()
            };

            if (numAudioSources <= 0) {
                numAudioSources = 1;
            }
        }

        void Start() {
            MGetCamera getCam = new MGetCamera();
            Main.msg.Dispatch(getCam);
            if (getCam.camera == null) {
                return;
            }

            Main.msg.RemoveListener<MCameraReady>(OnCameraReady);
            SetupCamera(getCam.camera);
        }

        private void OnCameraReady(MCameraReady msg) {
            SetupCamera(msg.camera);
        }

        private void SetupCamera(Camera cam) {
            Vector3 pos = new Vector3(
                transform.position.x,
                cam.transform.position.y,
                cam.transform.position.z + 10);

            audioSources = new AudioSource[numAudioSources];
            for (int i=0; i<numAudioSources; i++) {
                audioSources[i] = GO
                    .Create("AudioSource_"+i)
                    .SetParent(transform, false)
                    .SetPosition(pos)
                    .AddComponent<AudioSource>(OnInitAudioSource)
                    .GetComponent<AudioSource>();
            }

            GetOrderer(playOrder).Sort(sequence);
        }

        private void OnInitAudioSource(AudioSource source) {
            source.playOnAwake = false;
			source.minDistance = 12.5f;
            source.maxDistance = 27f;
            source.spatialBlend = 1f;
            source.dopplerLevel = 0f;
            source.spread = 0f;
			source.rolloffMode = AudioRolloffMode.Linear;
        }

        private IPlayOrder GetOrderer(PlayOrder po) {
            return orderers[(int)po];
        }

        public int GetNextClip() {
            // Store current clip
            int clip = curClip;
            // Increase the value to make it ready for the next call. If we
            // had simply increased the counter before returning it,
            // we would have to set the start value to -1 for it to start at 0
            curClip++;
            if (curClip == sequence.Length) {
                GetOrderer(playOrder).Sort(sequence);
                curClip = 0;
            }
            return clip;
        }

        public AudioSource GetNextAudioSource() {
            for (int i=0; i<audioSources.Length; i++) {
                if (!audioSources[i].isPlaying) {
                    return audioSources[i];
                }
            }
            return null;
        }

        public void Play(int s) {
            Play(GetNextAudioSource(), s);
        }

        public void Play() {
            Play(GetNextAudioSource(), GetNextClip());
        }

        public bool Play(AudioSource source, int numClip) {
            if (source == null) {
                return false;
            }

            int clip = sequence[numClip];
            source.outputAudioMixerGroup = output;
            source.volume = volume;
            source.clip = audioClips[clip];
            source.Play();

            return true;
        }
    }
}

