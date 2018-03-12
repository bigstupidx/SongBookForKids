using UnityEngine;

namespace Syng {
    public class SetPosition : MonoBehaviour {

        [SerializeField]
        private bool setWorldPos;

        public void OnSetPosition(Vector3 pos) {
            if (setWorldPos) {
                transform.position = pos;
            } else {
                transform.localPosition = pos;
            }
        }
    }
}
