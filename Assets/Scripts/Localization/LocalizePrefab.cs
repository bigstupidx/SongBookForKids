using UnityEngine;

namespace Syng {
    public class LocalizePrefab : MonoBehaviour {

        [SerializeField]
        private int         prefab;
        [SerializeField]
        private Transform   parent;

        void Awake() {
            if (parent == null) {
                parent = transform;
            }
        }

        public void OnLoadedPrefab(GameObject loaded, int i) {
            if (prefab != i) {
                return;
            }
            loaded.transform.parent = parent;
            loaded.transform.localPosition = Vector3.zero;
            loaded.SetActive(true);
        }

        public void OnUnloadPrefab(GameObject unload) {
        }
    }
}
