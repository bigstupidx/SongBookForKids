using UnityEngine;

namespace Syng {

    [AddComponentMenu("Syng/Effects/Distortion Sprite")]
    public class DistortionSprite : MonoBehaviour {

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
            block.SetTexture("_MainTex", block.GetTexture("_MainTex"));
            r.SetPropertyBlock(block);
        }

    }
}
