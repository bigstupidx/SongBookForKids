using UnityEngine;
using System;

namespace Syng {
    public class Eyes : MonoBehaviour, ICountdown {

        public AnimationCurve   lerpCurve;
        public Transform        lookTarget;
        public Transform        eyesOpen;
        public Transform        eyesClosed;
        public Transform        pupils;
        public float            blinkDuration;

        // Add help box to tell user to not set timer to a value less than
        // blinkduration

        private int             blinkState;
        private Action[]        states;
        private Countdown       countdown;

        void Awake() {
            countdown = new Countdown();
            countdown.OnCountdown.Add(this);

            // blinkState should default to 0 and be set to 2 from the inspector
            // when both open and close transform are set
            blinkState = eyesOpen == null || eyesClosed == null ? 0 : 2;
            states = new Action[] {
                Idle, CloseEyes, OpenEyes
            };
            states[blinkState]();
        }

        void Update() {
            countdown.Update();
            pupils.localPosition = lookTarget.localPosition;
        }

        // In the inspector: add verification when adding refs to eyes.
        // When you add both open/closed, notify that blinking is enabled.

        // sample curve over x seconds
        // use sampled value as speed multiplier
        // calculate speed using magnitude/length/distance

        public void OnCountdown() {
            // OpenEyes();
            blinkState = 2;
        }

        private void OpenEyes() {
            eyesOpen.gameObject.SetActive(true);
            eyesClosed.gameObject.SetActive(false);
            blinkState = 1;
        }

        private void CloseEyes() {
            eyesClosed.gameObject.SetActive(true);
            eyesOpen.gameObject.SetActive(false);
            countdown.Start(blinkDuration);
        }

        private void Idle() {}

        // public void OnBlink() {
        //     states[blinkState]();
        // }
    }
}
