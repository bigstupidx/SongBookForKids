using UnityEngine;

namespace Syng {

    [AddComponentMenu("Syng/Effects/Shadow Overlay")]
    public class ShadowOverlay : MonoBehaviour {

        public Texture2D    shadow;
        public Color        shadowColor;

        void Start() {
            SetMaterialProperties();
        }

        #if UNITY_EDITOR
        public void SetMaterialProperties() {
        #else
        private void SetMaterialProperties() {
        #endif

            Renderer r = GetComponent<Renderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            r.GetPropertyBlock(block);
            block.SetTexture("_ShadowTex",  shadow);
            block.SetColor("_ShadowColor",  shadowColor);
            r.SetPropertyBlock(block);
        }

    }
}
