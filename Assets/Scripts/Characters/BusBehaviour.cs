using UnityEngine;
using System.Collections;

namespace Syng {

    public class BusBehaviour : MonoBehaviour {

        public BezierCurve      curveFront;
        public BezierCurve      curveBack;
        public Transform        bus;
        public AnimationCurve   scale;
        public Transform[]      doors;
        public Transform[]      parts;
        public Transform[]      wheels;

        public BoolEvent        onDrive;

        private BusState[]      states;
        private BusState        curState;
        private int             curStateI;
        private float           time;
        private bool            ready;
        private bool            openingDoors;
        private float           baseScale;

        void Awake() {
            openingDoors = false;
            ready = false;
            time = 0.5f;
            baseScale = bus.localScale.x;

            states = new BusState[] {
                new BusState(curveFront,    10f),
                new BusState(curveBack,     10f),
                new BusState(curveFront,    8f),
                new BusState(curveBack,     8f),
                new BusState(curveFront,    6f, 0.5f),
            };
            curStateI = 0;
            curState = states[curStateI];
        }

        void Start() {
            curState.Move(time, transform);
        }

        void Update() {
            curState.Move(time, transform);
            if (!ready) {
                return;
            }

            time += Time.deltaTime / curState.speed;

            if (curStateI % 2 == 1) {
                bus.localScale = scale.Evaluate(time) * baseScale * Vector3.one;
            }

            for (int i=0; i<wheels.Length; i++) {
                float zrot = wheels[i].localRotation.eulerAngles.z;
                zrot -= Time.deltaTime * 120f;
                wheels[i].localRotation = Quaternion.Euler(Vector3.forward * zrot);
            }

            if (!curState.Move(time, transform)) {
                curStateI++;
                if (curStateI == states.Length) {
                    ready = false;
                    onDrive.Invoke(false);
                    return;
                }
                curState = states[curStateI];
                curState.Transition(bus, parts);
                time = 0f;
            }
        }

        public void Stop() {
            ready = false;
            time = 0.5f;
            curStateI = 0;
            curState = states[curStateI];
            curState.Move(time, transform);
            onDrive.Invoke(false);
            StopAllCoroutines();

            for (int i=0; i<doors.Length; i++) {
                doors[i].localRotation = Quaternion.Euler(Vector3.up);
            }

            for (int i=0; i<wheels.Length; i++) {
                wheels[i].localRotation = Quaternion.Euler(Vector3.forward);
            }

            bus.localScale = baseScale * Vector3.one;
        }

        public void SwingDoors() {
            if (openingDoors) {
                return;
            }
            StartCoroutine(OpenDoors(2));
        }

        public void Drive() {
            ready = true;
            curStateI = 0;
            curState = states[curStateI];
            onDrive.Invoke(true);
            StartCoroutine(DelayDoors());
        }

        IEnumerator DelayDoors() {
            yield return new WaitForSeconds(17f);
            StartCoroutine(OpenDoors(16));
        }

        IEnumerator OpenDoors(int repeats) {
            openingDoors = true;
            int waits = 0;
            while (waits < repeats) {
                float rot = waits % 2 == 0 ? 300f : 0f;
                rot *= curStateI % 2 == 0 ? 1 : -1;
                for (int i=0; i<doors.Length; i++) {
                    doors[i].localRotation = Quaternion.Euler(Vector3.up * rot);
                }
                yield return new WaitForSeconds(1f);
                waits++;
            }
            openingDoors = false;
        }
    }

    public interface IBusState {
        bool Move(float time, Transform bus);
        void Transition(Transform bus, Transform[] parts);
    }

    public class BusState : IBusState {
        public float            speed { get; private set; }

        private BezierCurve     curve;
        private float           done;

        public BusState(BezierCurve curve, float speed, float done = 0.99f) {
            this.curve = curve;
            this.speed = speed;
            this.done = done;
        }

        public bool Move(float time, Transform bus) {
            Vector3 point = curve.GetPointAt(time);
            point = curve.transform.InverseTransformPoint(point);
            point += curve.transform.localPosition;
            bus.localPosition = point;
            return time < done;
        }

        public void Transition(Transform bus, Transform[] parts) {
            float ry = bus.localRotation.eulerAngles.y;
            ry = ry > 0f ? 0f : 180f;
            bus.transform.localRotation = Quaternion.Euler(Vector3.up * ry);

            Vector3 p;
            for (int i=0; i<parts.Length; i++) {
                p = parts[i].localPosition;
                p.z *= -1;
                parts[i].localPosition = p;
            }
        }
    }
}
