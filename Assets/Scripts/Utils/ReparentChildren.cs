using UnityEngine;

namespace Syng {
    public class ReparentChildren : MonoBehaviour {

        [SerializeField]
        private Transform   reparentTo;
        [SerializeField]
        private Transform   reparentFrom;
        [SerializeField]
        private PlayOrder   order;

        private Sequence    sequence;
        private GetChildren getChildren;
        private Transform   currentChild;

        void Awake() {
            getChildren = new GetChildren(reparentFrom);
            sequence = new Sequence(getChildren.numChildren + 1, order);
        }

        public void Reparent() {
            if (currentChild != null) {
                MoveChild(currentChild, reparentFrom);
            }
            int n = sequence.GetNext();
            currentChild = n < getChildren.numChildren
                ? getChildren.GetChild(n)
                : null;
            if (currentChild != null) {
                MoveChild(currentChild, reparentTo);
            }
        }

        private void MoveChild(Transform child, Transform target) {
            child.parent = target;
            child.localPosition = Vector3.zero;
            child.rotation = target.rotation;
        }
    }
}
