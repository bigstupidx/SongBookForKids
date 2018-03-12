using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Mandarin;
using Mandarin.Plugins;
using Mandarin.PluginSystem;

namespace Syng {

    public interface IDayNight {
        void OnDayNight(float transition, float progress);
    }

    public interface ICameraMove {
        void OnCameraMove(float progress);
    }

    public class MGetCamera : IMessage {
        public Camera camera;
    }

    public class MCameraReady : IMessage {
        public Camera camera;
    }

    public class MAddDayNightHandler : IMessage {
        public IDayNight handler;
    }

    public class MAddCameraMoveHandler : IMessage {
        public ICameraMove handler;
    }

    [System.Serializable]
    public class DayNightEvent : UnityEvent<float, float> {}

    [System.Serializable]
    public class CameraMoveEvent : UnityEvent<float> {}

    [AddComponentMenu("Syng/Camera Rig")]
    [RegisterPlugin]
    public class CameraRigPlugin : MonoBehaviour, IPlugin, IUCUpdate {

        [SerializeField]
        private Camera              cam;
        [SerializeField]
        private DayNightEvent       onTransition;
        [SerializeField]
        private CameraMoveEvent     onCameraMove;
        [SerializeField]
        private float               boundsWidth;
        [SerializeField]
        private AnimationCurve      dayNightCycle;
        [SerializeField]
        private float               dayNight;
        private float               prevDayNight = -1f;
        private float               prevCamPos = -1f;
        private float               dayStart;
        private float               nightStart;

        private Signal<ICameraMove> onCameraMoveSig;
        private Signal<IDayNight>   onDayNight;
        private CameraPanning       panning;

        public void Init(Messenger msg) {
            onDayNight = new Signal<IDayNight>();
            msg.AddListener<MAddDayNightHandler>(OnAddDayNightHandler);
            onCameraMoveSig = new Signal<ICameraMove>();
            msg.AddListener<MAddCameraMoveHandler>(OnAddCamMoveHandler);
            msg.AddListener<MGetCamera>(OnGetCamera);
        }

        private void OnGetCamera(MGetCamera msg) {
            msg.camera = cam;
        }

        public void Ready(Messenger msg) {
            Assert.AreEqual(cam.orthographicSize, 11f,
                "Camera should have an orthographic size "+
                "of 11 due to the import settings of the "+
                "graphics.");

            float colliderHeight = cam.orthographicSize * 2f;
            float colliderWidth = colliderHeight * cam.aspect + boundsWidth;

            panning = GO.Create("CameraPanning")
                .AddComponent<CameraPanning>(cp => { cp
                    .Init(new CameraPanningConfig {
                        camera = cam,
                        tapThreshold = 1.5f,
                        scale = new Vector3(colliderWidth,
                                            colliderHeight,
                                            0f),
                        inertiaDuration = 2f
                    });
                })
                .SetLayer("Default")
                .SetParent(transform)
                .SetLocalPosition(new Vector3(0f, 0f, cam.transform.position.z + 1f))
                .GetComponent<CameraPanning>();

            dayStart = dayNightCycle.keys[1].time;
            nightStart = dayNightCycle.keys[2].time;

            msg.Dispatch(new MCameraReady {
                camera = cam
            });

            msg.Dispatch(new MAddUpdateHandler(this));
        }

        void Start() {
            // Should force update the day night transition to update dependencies
        }

        public void MoveCamera(float f) {
            Vector3 local = cam.transform.localPosition;
            local.x += boundsWidth / 2f;
            local.x /= boundsWidth;
            local.x = Mathf.Clamp01(local.x + f) * boundsWidth - boundsWidth / 2f;
            cam.transform.localPosition = local;
        }

        private void OnAddDayNightHandler(MAddDayNightHandler msg) {
            onDayNight.Add(msg.handler);
        }

        private void OnAddCamMoveHandler(MAddCameraMoveHandler msg) {
            onCameraMoveSig.Add(msg.handler);
        }

        public void UCUpdate() {
            Vector3 dt = Vector3.right * panning.deltaPosition.x;
            Vector3 local = cam.transform.localPosition;

            // returns 1 when there are no touches, 0 when there are
            int touches = (1 + (int)Mathf.Clamp01(panning.tapCount)) % 2;

            float overshootL = (local.x + dt.x) - boundsWidth * -.5f;
            float overshootLPct = Mathf.Clamp(overshootL / 6f, -1f, 0f);
            int toggleOvershootL = (int)Mathf.Floor(overshootLPct) * -1;
            // this will multiply by 1 when no overshoot
            // otherwise multiply delta by the inverse distance overshot in percent
            dt *= 1 - (overshootLPct * -1f);
            // multiply overshootL by 0.98 to cause a slowdown towards the end
            // of the lerp
            // multiply by deltaTime to lerp the overshoot distance over 1 second
            // multiply by touches to toggle lerping when pressing or releasing the
            // mouse button or finger
            // multiply by toggleOvershootL to enable the lerp only when overshooting
            dt += Vector3.right * overshootL * -2.5f * Time.deltaTime * touches * toggleOvershootL;

            float overshootR = (local.x + dt.x) - boundsWidth * .5f;
            float overshootRPct = Mathf.Clamp(overshootR / 6f, 0f, 1f);
            int toggleOvershootR = (int)Mathf.Ceil(overshootRPct);
            // this will multiply by 1 when no overshoot
            dt *= 1 - overshootRPct;
            // multiply overshootR by 0.98 to cause a slowdown towards the end
            // of the lerp
            // multiply by deltaTime to lerp the overshoot distance over 1 second
            // multiply by touches to toggle lerping when pressing or releasing the
            // mouse button or finger
            // multiply by toggleOvershootR to enable the lerp only when overshooting
            dt += Vector3.right * overshootR * -2.5f * Time.deltaTime * touches * toggleOvershootR;

            cam.transform.localPosition += dt;

            float posx = cam.transform.localPosition.x;
            float camPos = (posx + boundsWidth * .5f) / boundsWidth;
            if (prevCamPos != camPos) {
                onCameraMove.Invoke(camPos);
                int len = onCameraMoveSig.callbacks.Count;
                for (int i=0; i<len; i++) {
                    onCameraMoveSig.callbacks[i].OnCameraMove(camPos);
                }
                prevCamPos = camPos;
            }
            dayNight = dayNightCycle.Evaluate(Mathf.Clamp01(camPos));

            float progress = Mathf.Clamp01((camPos - dayStart) / (nightStart - dayStart));

            if (dayNight != prevDayNight) {
                onTransition.Invoke(dayNight, camPos);
                int len = onDayNight.callbacks.Count;
                for (int i=0; i<len; i++) {
                    onDayNight.callbacks[i].OnDayNight(dayNight, progress);
                }
                prevDayNight = dayNight;
            }
        }

        private Vector3 ClampX(Vector3 pos) {
            float x = Mathf.Clamp(pos.x,
                                  boundsWidth * -.5f,
                                  boundsWidth * .5f);
            return new Vector3(x, pos.y, pos.z);
        }

    }
}
