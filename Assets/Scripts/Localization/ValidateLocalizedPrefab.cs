using UnityEngine;

namespace Syng {
    public class ValidateLocalizedPrefab : MonoBehaviour {

        [SerializeField]
        private int prefab;
        [SerializeField]
        private GameObjectEvent onValid;

        public void OnValidatePrefab(GameObject go, int i) {
            if (i == prefab) {
                onValid.Invoke(go);
            }
        }
    }
}
