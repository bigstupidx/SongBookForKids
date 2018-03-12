using UnityEngine;
using Mandarin;

namespace Syng {
    public class ParticleBurst : MonoBehaviour {

        public ParticleSystem   particles;
        public Vector2          emissionMinMax;
        public float            sizeTarget;
        public float            speedTarget;
        public float            duration;
        public AnimationCurve   falloff;

        private float           endTime;
        private float           emissionMinOrig;
        private float           emissionMaxOrig;
        private float           sizeOrig;
        private float           speedOrig;
        private FSM             fsm;
        private ParticleSystem.EmissionModule   em;

        void Awake() {
            fsm = new FSM(2)
                .AddState(0, StateIdle)
                .AddState(1, StateBurst);

            em = particles.emission;
            emissionMinOrig = em.rate.constantMin;
            emissionMaxOrig = em.rate.constantMax;
            sizeOrig = particles.startSize;
            speedOrig = particles.startSpeed;
            endTime = 0f;
        }

        void Update() {
            fsm.Run();
        }

        public void Burst() {
            endTime = Time.time + duration;
            fsm.SetState(1);
        }

        private void StateIdle(FSM fsm) {}

        private void StateBurst(FSM fsm) {
            float progress = GetProgress(endTime, duration);
            float f = falloff.Evaluate(progress);

            float emMin = (f * emissionMinMax.x) + emissionMinOrig;
            float emMax = (f * emissionMinMax.y) + emissionMaxOrig;
            em.rate = new ParticleSystem.MinMaxCurve(emMin, emMax);

            particles.startSize = (f * sizeTarget) + sizeOrig;
            particles.startSpeed = (f * speedTarget) + speedOrig;

            fsm.SetState((int)Mathf.Ceil(1 - progress));
        }

        private float Map(float val, float mina, float maxa, float minb, float maxb) {
            return minb + (maxb - minb) * ((val - mina) / (maxa - mina));
        }

        // Returns a value that goes from 0 to 1 over the duration
        private float GetProgress(float end, float duration) {
            return 1f - Mathf.Clamp01((end - Time.time) / duration);
        }
    }
}
