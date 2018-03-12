using UnityEngine;
using Mandarin;
using Mandarin.PluginSystem;

namespace Syng {
    [RegisterPlugin]
    public class AmbientPlugin : MonoBehaviour, IPlugin, IDayNight {

        public AudioSource      audioSourceDay;
        public AudioSource      audioSourceNight;

        public AudioClip        ambientDay;
        public AudioClip        ambientNight;

        public AnimationCurve   curveCrossfade;

        public void Init(Messenger msg) {}

        public void Ready(Messenger msg) {
            msg.Dispatch(new MAddDayNightHandler {
                handler = this
            });
        }

        void Start() {
            audioSourceDay.clip = ambientDay;
            audioSourceDay.Play();
            audioSourceDay.volume = 1f;
            audioSourceNight.clip = ambientNight;
            audioSourceNight.volume = 0f;
            audioSourceNight.Play();
        }

        public void OnDayNight(float transition, float progress) {
            audioSourceDay.volume = curveCrossfade.Evaluate(1f - progress);
            audioSourceNight.volume = curveCrossfade.Evaluate(progress);
        }
    }
}
