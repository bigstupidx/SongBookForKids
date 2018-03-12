using UnityEngine;

namespace Syng {

    [AddComponentMenu("Syng/Effects/Gradient Overlay")]
    public class GradientOverlay : MonoBehaviour {

        public Texture2D    gradient;
        public Color        colorTop;
        public Color        colorBottom;

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
            block.SetTexture("_MainTex",    block.GetTexture("_MainTex"));
            block.SetTexture("_RampTex",    gradient);
            block.SetColor("_TopColor",     colorTop);
            block.SetColor("_BottomColor",  colorBottom);
            r.SetPropertyBlock(block);
        }

    }
}
