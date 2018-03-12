using UnityEngine;

namespace Syng {

    public class GetChildren {

        // [SerializeField]
        // private Transform       target;
        // [SerializeField]
        // private IntegerEvent    onGotChildren;

        private Transform[]     children;

        public GetChildren(Transform target) {
            // this.target = target;

            int childCount = target == null ? 0 : target.childCount;
            children = new Transform[childCount];

            for (int i=0; i<target.childCount; i++) {
                children[i] = target.GetChild(i);
            }

            // onGotChildren.Invoke(childCount);
        }

        public int numChildren {
            get { return children.Length; }
        }

        public Transform GetChild(int c) {
            return children[c];
        }
    }
}
