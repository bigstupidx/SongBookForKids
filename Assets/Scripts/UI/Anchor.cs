using UnityEngine;

namespace Syng {
    public class Anchor : MonoBehaviour {

        [SerializeField]
        private Vector3 offsetWorld;
        [SerializeField]
        private Vector3 viewportPos;

        public void OnAnchor(Vector3 relativeTo) {
            float x = 1f - (viewportPos.x / Camera.main.aspect);
            float y = 1f - viewportPos.y;
            Vector3 scaledViewPos = new Vector3(x, y, 0f);
            Vector3 pos = Camera.main.ViewportToWorldPoint(scaledViewPos);
            pos = Camera.main.transform.position - pos;
            pos.z = transform.position.z;
            transform.position = pos + relativeTo + offsetWorld;
        }
    }
}
