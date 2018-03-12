using UnityEngine;

namespace Syng {

    [RequireComponent(typeof(BoxCollider))]
    public class ResizeBoxCollider : MonoBehaviour {

        private BoxCollider bc;

        void Awake() {
            bc = GetComponent<BoxCollider>();
        }

        public void OnResize(Bounds bounds) {
            bc.size = bounds.size;
        }
    }
}
