using UnityEngine;
using UnityEngine.UI;

namespace Syng {
    public class ScrollTo : MonoBehaviour {

        [SerializeField]
        private ScrollRect  scrollRect;
        [SerializeField]
        private float       position;

        public void Scroll() {
            scrollRect.verticalNormalizedPosition = position;
        }

    }
}
