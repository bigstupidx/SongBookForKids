using UnityEngine;

namespace Syng {
    public class GradientLerp : MonoBehaviour {

        [SerializeField]
        private Gradient[]              gradients;

        [SerializeField]
        private float[]                 thresholds;

        private new MeshRenderer        renderer;
        private MaterialPropertyBlock   block;
        private Color                   colorTop;
        private Color                   colorBottom;
        private int                     prev;
        private int                     next;
        private float                   lower;
        private float                   upper;
        private int                     upperThreshold;

        void Awake() {
            renderer = GetComponent<MeshRenderer>();
            block = new MaterialPropertyBlock();

            upperThreshold = thresholds.Length - 1;
            prev = 0;
            next = 1;
            lower = thresholds[prev];
            upper = thresholds[next];
        }

        public void OnCameraMove(float t) {
            t = Mathf.Clamp01(t);

            if (t < lower) {
                if (prev > 0) {
                    prev--;
                }
                if (next > 0) {
                    next--;
                }
            }
            if (t > upper) {
                if (prev < upperThreshold) {
                    prev++;
                }
                if (next < upperThreshold) {
                    next++;
                }
            }

            lower = thresholds[prev];
            upper = thresholds[next];

            Gradient gradientFrom = gradients[prev];
            Gradient gradientTo = gradients[next];

            float range = upper - lower;
            float sample = (t - lower) / range;
            colorTop = Color.Lerp(gradientFrom.Evaluate(0f),
                                  gradientTo.Evaluate(0f),
                                  sample);
            colorBottom = Color.Lerp(gradientFrom.Evaluate(1f),
                                     gradientTo.Evaluate(1f),
                                     sample);

            renderer.GetPropertyBlock(block);
            block.SetColor("_TopColor",     colorTop);
            block.SetColor("_BottomColor",  colorBottom);
            renderer.SetPropertyBlock(block);
        }
    }
}
