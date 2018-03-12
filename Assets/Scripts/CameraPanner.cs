using UnityEngine;
using System.Collections;

namespace Syng {

    [RequireComponent(typeof(CameraRigPlugin))]
    public class CameraPanner : MonoBehaviour {

        public float            secPerUnit;
        public float            delay;

        private CameraRigPlugin rig;
        private bool            move;

        void OnEnable() {
            rig = GetComponent<CameraRigPlugin>();
            StartCoroutine(WaitASecond(delay));
        }

        IEnumerator WaitASecond(float sec) {
            yield return new WaitForSeconds(sec);
            move = true;
        }

        void Update() {
            if (!move) {
                return;
            }
            rig.MoveCamera(Time.deltaTime / secPerUnit);
        }
    }
}
