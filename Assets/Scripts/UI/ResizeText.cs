using UnityEngine;
using UnityEngine.UI;

namespace Syng {
    [RequireComponent(typeof(RectTransform))]
    public class ResizeText : MonoBehaviour {

        void Start() {
            RectTransform rt = transform.GetComponent<RectTransform>();
            float height = LayoutUtility.GetPreferredHeight(rt);
            rt.sizeDelta = Vector2.up * height;
        }

    }
}
