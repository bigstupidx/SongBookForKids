using UnityEngine;
using System;
using Mandarin;

// It should be self contained. Either use UnityCallbacks, a threaded Timer,
// or some other magic. NO COROUTINE DAMMIT!

// If I use a threaded timer, Countdown needs to clean up the timer.
// It needs to listen to application quit, app pause. StopTimer needs to cleanup
// too. Restarting too.

namespace Syng {

    public interface ICountdown {
        void OnCountdown();
    }

    public class Countdown {

        public Signal<ICountdown>   OnCountdown;

        private float               countdown;
        private Action[]            states;
        private int                 state;

        public Countdown() {
            state = 0;
            states = new Action[] { Idle, Counting };
            OnCountdown = new Signal<ICountdown>();
        }

        public void Start(float duration) {
            countdown = duration;
            state = 1;
        }

        public void Update() {
            states[state]();
        }

        private void Idle() {}

        private void Counting() {
            countdown -= Time.deltaTime;
            if (countdown <= 0) {
                state = 0;
                OnCountdown.Invoke(c => { c.OnCountdown(); });
            }
        }
    }
}
