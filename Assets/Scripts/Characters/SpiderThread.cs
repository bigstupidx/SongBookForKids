using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Mandarin;

namespace Syng {

    [System.Serializable]
    public class ThreadPluck : UnityEvent<int> {}

    public class SpiderThread : MonoBehaviour {

        [Min(1)]
        public int                  zones;
        public AnimationCurve       pluckFalloff;
        public float                pluckDuration;
        public int                  defaultPluckZone;
        public Sprite[]             sprites;
        public ThreadPluck          onPluck;

        private SpriteRenderer      spriteRenderer;
        private Vector2             downPos;
        private FSM                 fsm;
        private float               endTime;

        void Awake() {
            fsm = new FSM(2)
                .AddState(0, Idle)
                .AddState(1, Flip);

            endTime = 0f;
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Resize the collider to wrap the sprites
            Sprite sprite = sprites[0];
            BoxCollider bc = GetComponent<BoxCollider>();
            bc.size = new Vector3(sprite.bounds.size.x * 2,
                                  sprite.bounds.size.y,
                                  0f);
            fsm.SetState(0);
        }

        void Update() {
            fsm.Run();
        }

        public float GetZoneHeight(SpriteRenderer sr, int numZones) {
            float height = sr.bounds.size.y;
            return height / numZones;
        }

        public void Pluck() {
            Play();
            onPluck.Invoke(defaultPluckZone);
        }

        public void Pluck(PointerEventData data) {
            Play();

            float zoneHeight = GetZoneHeight(spriteRenderer, zones);
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(data.position);
            float top = transform.position.y - spriteRenderer.bounds.extents.y;
            float clickY = clickPos.y - top;
            int zone = (int)Mathf.Floor(clickY / zoneHeight);
            onPluck.Invoke(zone);
        }

        private void Play() {
            endTime = Time.time + pluckDuration;
            spriteRenderer.flipX = false;
            fsm.SetState(1);
        }

        // Returns a value that goes from 0 to 1 over the duration
        private float GetProgress(float end, float duration) {
            return 1f - Mathf.Clamp01((end - Time.time) / duration);
        }

        private float GetFalloff(float progress, AnimationCurve curve) {
            return curve.Evaluate(progress);
        }

        private int GetFrame(float progress, float duration, int numFrames) {
            float frameSpan = 1f / numFrames;
            return (int)Mathf.Floor(progress * duration / frameSpan);
        }

        private void Idle(FSM fsm) {}

        private void Flip(FSM fsm) {
            spriteRenderer.flipX = !spriteRenderer.flipX;

            float progress = GetProgress(endTime, pluckDuration);
            progress = GetFalloff(progress, pluckFalloff);
            int frame = GetFrame(progress, pluckDuration, sprites.Length);

            spriteRenderer.sprite = sprites[frame];
            fsm.SetState((int)Mathf.Clamp01(frame));
        }
    }
}
