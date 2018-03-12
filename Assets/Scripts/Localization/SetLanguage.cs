using UnityEngine;

namespace Syng {
    public class SetLanguage : MonoBehaviour {

        [SerializeField]
        private int             language;
        [SerializeField]
        private IntegerEvent    onSetLanguage;

        public void OnSetLanguage() {
            onSetLanguage.Invoke(language);
        }
    }
}
