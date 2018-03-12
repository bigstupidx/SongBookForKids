using UnityEngine;
using UnityEngine.Events;

namespace Syng {

    public class CreateInteger : MonoBehaviour {

        public int              minValue;
        public int              maxValue;
        public IntegerEvent     onCreate;

        public void Create() {
            onCreate.Invoke(Random.Range(minValue, maxValue + 1));
        }
    }
}
