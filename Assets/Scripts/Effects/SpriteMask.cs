using UnityEngine;

namespace Syng {

    [AddComponentMenu("Syng/Effects/Sprite Mask")]
    public class SpriteMask : MonoBehaviour {

        public Texture2D    mask;
        [Range(0f, 1f)]
        public float        offsetX;
        [Range(0f, 1f)]
        public float        offsetY;

        void Start() {
            SetMaterialProperties();
        }

        #if UNITY_EDITOR
        public void SetMaterialProperties() {
        #else
        private void SetMaterialProperties() {
        #endif

            SpriteRenderer r = GetComponent<SpriteRenderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            r.GetPropertyBlock(block);
            block.SetTexture("_MainTex",    block.GetTexture("_MainTex"));
            block.SetColor("_Color",        r.color);
            block.SetTexture("_MaskTex",    mask);
            block.SetFloat("_OffsetX",      offsetX);
            block.SetFloat("_OffsetY",      offsetY);
            r.SetPropertyBlock(block);
        }

    }
}
