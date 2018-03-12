using UnityEngine;

namespace Syng {
    public class ColorLerp : MonoBehaviour {

        [SerializeField]
        private Color                   colorFrom = new Color(1f, 1f, 1f, 1f);
        [SerializeField]
        private Color                   colorTo = new Color(1f, 1f, 1f, 1f);

        private new MeshRenderer        renderer;
        private MaterialPropertyBlock   block;

        void Awake() {
            renderer = GetComponent<MeshRenderer>();
            block = new MaterialPropertyBlock();
        }

        public void OnDayNight(float t, float p) {
            SetColor(GetColorAt(t), renderer, block);
        }

        public Color GetColorAt(float t) {
            return Color.Lerp(colorFrom, colorTo, t);
        }

        public void SetColor(Color                  color,
                             MeshRenderer           renderer,
                             MaterialPropertyBlock  block) {
            renderer.GetPropertyBlock(block);
            block.SetColor("_Color", color);
            renderer.SetPropertyBlock(block);
        }
    }
}
