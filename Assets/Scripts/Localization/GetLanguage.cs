using UnityEngine;
using UnityEngine.Events;

namespace Syng {
    public class GetLanguage : MonoBehaviour {

        [SerializeField]
        private int             language;
        [SerializeField]
        private UnityEvent      onGotLanguage;
        [SerializeField]
        private UnityEvent      onGotOtherLanguage;

        public void OnGetLanguage(int lang) {
            if (lang != language) {
                onGotOtherLanguage.Invoke();
                return;
            }
            onGotLanguage.Invoke();
        }

    }
}
