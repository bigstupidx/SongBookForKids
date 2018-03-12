using UnityEngine;

namespace Syng {
    public class MoveTransform : MonoBehaviour {

        [SerializeField]
        private Transform   target;
        [SerializeField]
        private Transform   moveTo;

        public void Move() {
            if (target.parent != moveTo) {
                target.parent = moveTo;
                target.localPosition = Vector3.zero;
                target.rotation = moveTo.rotation;
            }
        }

    }
}
