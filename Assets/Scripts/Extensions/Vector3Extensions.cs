using UnityEngine;

namespace Mandarin {
    static public partial class Vector3Extensions {

        static public Vector3 Round(this Vector3 v3) {
            v3.x = Mathf.Round(v3.x);
            v3.y = Mathf.Round(v3.y);
            v3.z = Mathf.Round(v3.z);
            return v3;
        }

        static public Vector3 Sign(this Vector3 v3) {
            return new Vector3(M.Sign(v3.x),
                               M.Sign(v3.y),
                               M.Sign(v3.z));
        }

        static public Vector3 Abs(this Vector3 v) {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.z = Mathf.Abs(v.z);
            return v;
        }

        static public Vector3 Floor(this Vector3 v) {
            v.x = Mathf.Floor(v.x);
            v.y = Mathf.Floor(v.y);
            v.z = Mathf.Floor(v.z);
            return v;
        }

        static public Vector3 Ceil(this Vector3 v) {
            v.x = Mathf.Ceil(v.x);
            v.y = Mathf.Ceil(v.y);
            v.z = Mathf.Ceil(v.z);
            return v;
        }

        static public Vector3 Divide(this Vector3 a, Vector3 b) {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        static public Vector3 Divide(this Vector3 a, float b) {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        static public float Product(this Vector3 v) {
            return v.x * v.y * v.z;
        }
    }
}
