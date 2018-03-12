using UnityEngine;
using UnityEngine.Events;

namespace Syng {
    public class Branch : MonoBehaviour {

        public UnityEvent   onTrue;
        public UnityEvent   onFalse;

        public void Evaluate(bool expression) {
            if (expression) {
                onTrue.Invoke();
            } else {
                onFalse.Invoke();
            }
        }
    }
}
