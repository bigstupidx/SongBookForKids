using UnityEngine;
using UnityEngine.UI;

namespace Syng {

    [RequireComponent(typeof(Image))]
    public class LocalizeImage : MonoBehaviour {

        [SerializeField]
        private Sprite[]    sprites;
        private Image       image;

        void Awake() {
            image = GetComponent<Image>();
        }

        public void OnLocalizeImage(int language) {
            image.sprite = sprites[language];
        }

    }
}
