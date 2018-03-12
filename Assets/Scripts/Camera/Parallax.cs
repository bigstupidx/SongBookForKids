using UnityEngine;
using System.Collections.Generic;

namespace Syng {

    [AddComponentMenu("Syng/Camera/Parallax")]
    [RequireComponent(typeof(Camera))]
    public class Parallax : MonoBehaviour {

        [SerializeField]
        private int             zones = 4;
        [SerializeField]
        private AnimationCurve  speedCurve = new AnimationCurve();
        [SerializeField]
        private string          tagName;
        [SerializeField]
        private int[]           zoneMap = new int[0];
        [SerializeField]
        private Transform[]     elements = new Transform[0];

        private Vector3         lastPos;

        void Start() {
            lastPos = transform.position;
            IndexParallaxElements();
        }

        void Update() {
            UpdateElements(transform.position - lastPos);
            lastPos = transform.position;
        }

        private void UpdateElements(Vector3 dt) {
            int zone = 0;
            for (int i=0; i<elements.Length; i++) {
                Transform t = elements[i];
                float sample = GetZoneSamplePoint(zone);
                Vector3 dir = t.InverseTransformDirection(dt * speedCurve.Evaluate(sample));
                t.Translate(dir);

                int zoneThreshold = zoneMap[zone];
                // Bit shifting trick for returning the positive
                // sign of a number. When (i - zoneThreshold) is < 0,
                // sign is 0, otherwise +1.
                // As long as the sign is 0 we keep sampling from the
                // same zone. When the value of i is the same as
                // the zone threshold, the sign returns 1 and
                // we advance to the next zone.
                zone += ((i - zoneThreshold) >> 31) + 1;
            }
        }

        public float GetZoneSamplePoint(int zone) {
            return ((float)zone / (float)zones) + (1f / zones) * .5f;
        }

        public void IndexParallaxElements() {
            if (tagName == "") {
                Debug.LogError("Please supply a tag");
                return;
            }

            Camera cam = GetComponent<Camera>();

            ClearArrayElements();

            List<GameObject> objs = new List<GameObject>(
                GameObject.FindGameObjectsWithTag(tagName)
            );

            // Sort tagged GameObjects by their distance along z from camera.
            // First element in list is the one furthest away
            objs.Sort((a, b) => {
                Vector3 campos = cam.transform.position;
                Vector3 apos = campos + Vector3.forward * a.transform.position.z;
                Vector3 bpos = campos + Vector3.forward * b.transform.position.z;
                return Vector3.Distance(campos, bpos)
                        .CompareTo(Vector3.Distance(campos, apos));
            });

            float zoneSize = (cam.farClipPlane - cam.nearClipPlane) / (float)zones;
            float curZoneLimit = cam.nearClipPlane + zoneSize + cam.transform.position.z;
            int higher = 0;

            List<Transform> elms = new List<Transform>();
            List<int> map = new List<int>();

            // Iterate the zones and count the GameObjects within their
            // thresholds. Empty zones will be registered too. The number
            // of elements within a zone will be appended to a total
            // and added to the zoneMap list.
            for (int zone=0; zone<zones; zone++) {
                int count = higher;
                for (int i=count; i<objs.Count; i++) {
                    GameObject go = objs[i];

                    if (go.transform.position.z >= curZoneLimit) {
                        break;
                    }

                    elms.Add(objs[i].transform);
                    count++;
                }

                higher = count;
                map.Add(higher - 1);
                curZoneLimit += zoneSize;
            }

            elements = elms.ToArray();
            zoneMap = map.ToArray();
        }

        public void ClearArrayElements() {
            elements = new Transform[0];
            zoneMap = new int[0];
        }

    }
}
