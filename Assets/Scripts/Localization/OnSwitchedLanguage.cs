using UnityEngine;

namespace Syng {
    public class OnSwitchedLanguage : MonoBehaviour {

        [SerializeField]
        private IntegerEvent onSwitchedLanguage;

        void Awake() {
            Main.msg.AddListener<MLanguageAssetsLoaded>(OnLoadedLang);
        }

        private void OnLoadedLang(MLanguageAssetsLoaded languageAssetsLoaded) {
            onSwitchedLanguage.Invoke(languageAssetsLoaded.language);
        }
    }
}
