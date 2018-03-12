using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Syng {

    public class DandelionBehaviour : MonoBehaviour {

        public float                speedUpper = 2f;
        public float                speedLower = 1f;
        public CurveIndex           curveIndex;
        public DandelionSeed[]      seeds;
        public UnityEvent           onDrift;

        private int                 readySeeds;
        private int                 curCurve;
        private BezierCurve[]       curves;
        private DriftingComparer    comparer;

        void Awake() {
            comparer = new DriftingComparer();
            curCurve = 0;
            readySeeds = seeds.Length;
        }

        void Start() {
            for (int i=0; i<seeds.Length; i++) {
                seeds[i].SetOwner(this);
            }
        }

        public void DriftMuteEvent() {
            AnimateSeed();
        }

        public void Drift() {
            AnimateSeed();
            onDrift.Invoke();
        }

        private void AnimateSeed() {
            if (readySeeds == 0) {
                return;
            }

            readySeeds--;
            if (curCurve == 0) {
                curves = curveIndex.GetRandomizedIndex();
            }

            Array.Sort(seeds, comparer);
            DandelionSeed seed = seeds[0];
            curves[curCurve].transform.position = seed.transform.position;
            seed.Drift(curves[curCurve],
                       UnityEngine.Random.Range(speedLower, speedUpper));

            curCurve = (curCurve + 1) % curves.Length;
        }

        public void OnSeedPopped() {
            readySeeds++;
        }
    }

    public class DriftingComparer : IComparer<DandelionSeed> {
        public int Compare(DandelionSeed a, DandelionSeed b) {
            if (a.IsDrifting() && !b.IsDrifting()) {
                return 1;
            }
            if (!a.IsDrifting() && b.IsDrifting()) {
                return -1;
            }
            return 0;
        }
    }
}
