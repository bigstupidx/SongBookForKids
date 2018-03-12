using UnityEngine;
using UnityEngine.Events;

namespace Syng {

    [RequireComponent(typeof(SpriteRenderer))]
    public class LocalizeSprite : MonoBehaviour {

        [SerializeField]
        private Sprite[]                sprites;
        [SerializeField]
        private SpriteRendererEvent     onLocalizedSprite;
        private SpriteRenderer          sr;

        void Awake() {
            sr = GetComponent<SpriteRenderer>();
        }

        public void OnLocalizeSprite(int language) {
            sr.sprite = sprites[language];
            onLocalizedSprite.Invoke(sr);
        }

    }
}
