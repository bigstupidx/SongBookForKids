using System;
using UnityEngine;
using UnityEngine.Events;

// TODO: When should the timer start? On Start? Hook up to another event?
// Could use the visible message on PetterEdderkopp to dispatch a visible message

namespace Syng {
    [AddComponentMenu("Utils/Timer")]
    public class Timer : MonoBehaviour, ICountdown {

        public TimerMode        mode;

        [SerializeField]
        private float           fixedInterval;
        [SerializeField]
        private float           randomMin;
        [SerializeField]
        private float           randomMax;
        [SerializeField]
        private bool            autoStart;

        // 0 or less = infinity
        [SerializeField]
        private int             repeatMax;

        [SerializeField]
        private UnityEvent      onTimer;

        private Countdown       countdown;
        private Func<float>[]   counterFns;
        private Func<float>     getInterval;
        private int             repeatCount;

        #if UNITY_EDITOR
        [SerializeField]
        private float           curTime;
        #endif

        void Awake() {
            countdown = new Countdown();
            countdown.OnCountdown.Add(this);
            counterFns = new Func<float>[] { GetFixedInterval, GetRandomRange };

            // Won't allow us to change mode at runtime
            // Put it in StartTimer()?
            getInterval = counterFns[(int)mode];
        }

        void Start() {
            if (autoStart) {
                StartTimer();
            }
        }

        void Update() {
            countdown.Update();
        }

        public void OnCountdown() {
        // private void OnTime() {
            onTimer.Invoke();
            if (repeatMax <= 0) {
                StartTimer();
                return;
            }
            repeatCount++;
            if (repeatCount < repeatMax) {
                StartTimer();
            }
        }

        private float GetFixedInterval() {
            return fixedInterval;
        }

        private float GetRandomRange() {
            return UnityEngine.Random.Range(randomMin, randomMax);
        }

        private void StartTimer() {
            #if UNITY_EDITOR
            curTime = getInterval();
            countdown.Start(curTime);
            #else
            countdown.Start(getInterval());
            #endif
        }

        // Hook up to an event to start timer manually
        public void OnTimerReady() {
            StartTimer();
        }

        // TODO: Add a public stop method
    }
}
