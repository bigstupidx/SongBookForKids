using UnityEngine;

namespace Syng {
    public class Vector3Array : MonoBehaviour {

        [SerializeField]
        private Vector3[]       vector3Array;
        [SerializeField]
        private Vector3Event    onSelectedVector3;

        public void SelectVector3(int i) {
            if (i < 0 || i >= vector3Array.Length) {
                return;
            }
            onSelectedVector3.Invoke(vector3Array[i]);
        }
    }
}
