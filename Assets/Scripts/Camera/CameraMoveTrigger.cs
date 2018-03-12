using UnityEngine;
using UnityEngine.Events;

namespace Syng {
    public class CameraMoveTrigger : MonoBehaviour, ICameraMove {

        public float        threshold;
        public UnityEvent   onCameraMove;

        private bool        didMove;

        void Start() {
            didMove = false;
        }

        public void OnCameraMove(float progress) {
            bool moved = progress > threshold;
            if (moved && !didMove) {
                didMove = true;
                onCameraMove.Invoke();
            }
        }
    }
}
