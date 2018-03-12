using UnityEngine;
using UnityEngine.Events;

namespace Syng {
    [RequireComponent(typeof(SpriteRenderer))]
    public class Flipbook : MonoBehaviour {

        [SerializeField]
        private Sprite[]            sprites;
        [SerializeField]
        private int                 fps = 12;
        [SerializeField]
        [Tooltip("Set to 0 or a negative value to loop indefinitely")]
        private int                 repeats = 1;
        [SerializeField]
        private bool                autoPlay = false;
        [SerializeField]
        private UnityEvent          onAnimDone;
        [SerializeField]
        private UnityEvent          onAnimReplay;

        private new SpriteRenderer  renderer;
        private float               nextFrameTime;
        private int                 curFrame;
        private bool                paused;
        private int                 playTimes;

        void Awake() {
            paused = !autoPlay;
            renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = null;
            if (sprites.Length == 0) {
                return;
            }

            if (autoPlay) {
                Play();
            }
        }

        void Update() {
            if (paused) {
                return;
            }

            nextFrameTime -= Time.deltaTime;
            if (nextFrameTime <= 0f) {
                UpdateNextFrameTime();
                int cur = curFrame;
                SetFrame(curFrame + 1);
                Render();
                if (curFrame > cur) {
                    return;
                }
                if (repeats <= 0) {
                    onAnimReplay.Invoke();
                    return;
                }
                playTimes++;
                if (playTimes >= repeats) {
                    paused = true;
                    SetFrame(sprites.Length - 1);
                    Render();
                    onAnimDone.Invoke();
                }
            }
        }

        public void Play() {
            paused = false;
            UpdateNextFrameTime();
            SetFrame(0);
            Render();
        }

        public void Pause() {
            paused = true;
        }

        public void Stop() {
            paused = true;
            renderer.sprite = null;
        }

        public void FreezeFrame(int f) {
            paused = true;
            SetFrame(f);
            Render();
        }

        private void UpdateNextFrameTime() {
            nextFrameTime = 1f / fps;
        }

        private void SetFrame(int f) {
            curFrame = f;
            curFrame %= sprites.Length;
        }

        private void Render() {
            renderer.sprite = sprites[curFrame];
        }

        public int numFrames {
            get { return sprites.Length; }
        }

    }
}
