using UnityEngine;

namespace Syng {
    public class CreateVector3 : MonoBehaviour {

        [SerializeField]
        private Vector3         vector3;
        [SerializeField]
        private Vector3Event    onVector3Created;

        void Start() {
            onVector3Created.Invoke(vector3);
        }
    }
}
