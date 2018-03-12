using UnityEngine;
using UnityEngine.Assertions;

namespace Syng {

    [AddComponentMenu("Syng/Effects/Tint Material")]
    public class MaterialTinter : MonoBehaviour {

        public Material material;
        public Gradient colorRamp;

        void Awake() {
            Assert.IsNotNull<Material>(material,
                "MaterialTinter will fail since the material field "+
                "is null.");
        }

        void OnApplicationQuit() {
            material.color = colorRamp.Evaluate(0f);
        }

        public void Tint(float tint) {
            material.color = colorRamp.Evaluate(tint);
        }
    }
}
