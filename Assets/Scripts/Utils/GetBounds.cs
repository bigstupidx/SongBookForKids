using UnityEngine;

namespace Syng {
    public class GetBounds : MonoBehaviour {

        [SerializeField]
        private BoundsEvent onGotBounds;

        public void GotBounds(SpriteRenderer sr) {
            onGotBounds.Invoke(sr.bounds);
        }

        public void GotBounds(MeshRenderer mr) {
            onGotBounds.Invoke(mr.bounds);
        }
    }
}
