using UnityEngine;
using UnityEngine.UI;

namespace Syng {
    public class SetImage : MonoBehaviour {

        [SerializeField]
        private Image   image;
        [SerializeField]
        private Sprite  sprite;

        public void OnSetImage() {
            image.sprite = sprite;
        }
    }
}
