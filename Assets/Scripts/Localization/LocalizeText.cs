using UnityEngine;
using UnityEngine.UI;

namespace Syng {

    public class LocalizeText : MonoBehaviour {

        [SerializeField]
        private TextAsset[] texts;
        [SerializeField]
        private Text        textRenderer;

        public void OnLocalizeText(int language) {
            textRenderer.text = texts[language].text;
        }

    }
}
