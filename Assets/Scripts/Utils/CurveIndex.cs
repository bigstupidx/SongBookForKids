using UnityEngine;

namespace Syng {
    public class CurveIndex : MonoBehaviour {

        private BezierCurve[] curves;

        void Start() {
            curves = new BezierCurve[transform.childCount];

            for (int i=0; i<transform.childCount; i++) {
                curves[i] = transform.GetChild(i).GetComponent<BezierCurve>();
            }
        }

        public BezierCurve[] GetRandomizedIndex() {
            for (int i=0; i<curves.Length; i++) {
                int r = Random.Range(i, curves.Length-1);
                BezierCurve current = curves[i];
                curves[i] = curves[r];
                curves[r] = current;
            }
            return curves;
        }

    }
}
