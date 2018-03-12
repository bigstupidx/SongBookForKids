using UnityEngine;
using UnityEngine.EventSystems;

namespace Syng {
    public class BonfireBehaviour : MonoBehaviour, IPointerClickHandler {

        public float            intensity;
        // public float            intensity { get; private set; }

        public ParticleSystem   particles;
        public float            forceMax;
        public float            radiusMax;
        public float            rateMax;
        public float            sizeMax;

        public FloatEvent       onFire;

        private float           forceMin;
        private float           radiusMin;
        private Vector3         posStart;
        private float           rateMin;
        private float           sizeMin;

        void Start() {
            intensity = 0f;
            radiusMin = particles.shape.radius;
            forceMin = particles.forceOverLifetime.y.constant;
            posStart = particles.transform.localPosition;
            rateMin = particles.emission.rate.constant;
            sizeMin = particles.startSize;
        }

        void Update() {
            var fol = particles.forceOverLifetime;
            fol.y = new ParticleSystem.MinMaxCurve(Lerp(forceMin,
                                                        forceMax,
                                                        intensity));

            var shape = particles.shape;
            shape.radius = Lerp(radiusMin, radiusMax, intensity);

            Vector3 pos = posStart + Vector3.up * intensity * (radiusMax - radiusMin);
            particles.transform.localPosition = pos;

            var emission = particles.emission;
            emission.rate = new ParticleSystem.MinMaxCurve(Lerp(rateMin,
                                                                rateMax,
                                                                intensity));

            particles.startSize = Lerp(sizeMin, sizeMax, intensity);

            onFire.Invoke(intensity);
            intensity *= 0.99f;
            intensity = Mathf.Clamp01(intensity);
        }

        private float Lerp(float min, float max, float t) {
            return (max - min) * t + min;
        }

        public void OnPointerClick(PointerEventData data) {
            intensity = 1f;
        }
    }
}
