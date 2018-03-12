using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace Syng {

    public class HitComparer : IComparer<RaycastHit> {

        public int Compare(RaycastHit a, RaycastHit b) {
            if (a.distance > b.distance) {
                return -1;
            }
            return a.distance < b.distance ? 1 : 0;
        }
    }

    public class CameraPanningConfig {
        public float    inertiaDuration = 2f; // sec
        public Camera   camera;
        public float    tapThreshold = 0.5f;
        public Vector3  scale;
    }

    [RequireComponent(typeof(BoxCollider))]
    public class CameraPanning : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

        public Vector2              deltaPosition { get; private set; }
        public int                  tapCount { get; private set; }

        private float               inertiaDuration;
        private float               tapThreshold;
        private Vector2             prevPosition;
        private Vector2             curPosition;
        private BoxCollider         bc;
        private float               scalingFactor;
        private PointerEventData    eventData;
        private float               inertiaTime;
        private Vector3             tapPos;
        private Camera              cam;
        private RaycastHit[]        results;
        private LayerMask           layerDefault;
        private HitComparer         hitComparer;

        void Awake() {
            hitComparer = new HitComparer();
            tapCount = 0;
            layerDefault = 1 << LayerMask.NameToLayer("Interactive");
            inertiaTime = 0f;
            eventData = null;
            scalingFactor = 1f;
            deltaPosition = Vector2.zero;
            results = new RaycastHit[5];
        }

        void Update() {
            float inertia = (Time.time - inertiaTime) / inertiaDuration;
            deltaPosition = Vector2.Lerp(deltaPosition, Vector2.zero, inertia);

            if (ValidInput(eventData)) {
                curPosition = eventData.position;
                deltaPosition = (prevPosition - curPosition) * scalingFactor;
                prevPosition = curPosition;
            }

            if (tapCount > 0) {
                Raycast(eventData, pointerHold);
            }
        }

        public void Init(CameraPanningConfig cfg) {
            cam = cfg.camera;
            scalingFactor = cam.orthographicSize / cam.pixelHeight * 2f;
            tapThreshold = cfg.tapThreshold * scalingFactor;
            inertiaDuration = cfg.inertiaDuration;
            bc = GetComponent<BoxCollider>();
            bc.size = cfg.scale;
        }

        private bool ValidInput(PointerEventData data) {
            if (data == null) {
                return false;
            }
            return data.pointerId == 0 ||
                   data.button == PointerEventData.InputButton.Left;
        }

        public void OnPointerDown(PointerEventData data) {
            tapCount = data.button == PointerEventData.InputButton.Left
                ? 1
                : Input.touchCount;

            if (tapCount > 1) {
                return;
            }

            if (ValidInput(data)) {
                tapPos = cam.ScreenToWorldPoint(data.position)
                         - cam.transform.position;
                inertiaTime = Time.time;
                eventData = data;
                prevPosition = data.position;
                curPosition = data.position;
                Raycast(data, pointerDown);
                data.Use();
            }
        }

        public void OnPointerUp(PointerEventData data) {
            tapCount = data.button == PointerEventData.InputButton.Left
                ? 0
                : Input.touchCount;

            if (ValidInput(data)) {
                curPosition = Vector2.zero;
                prevPosition = Vector2.zero;
                eventData = null;
                Vector3 dragDist = ToWorldPos(data.position)
                                   - cam.transform.position
                                   - tapPos;

                Raycast(data, pointerUp);

                if (dragDist.magnitude * scalingFactor < tapThreshold) {
                    // Raycast into the scene and look for tappable objects.
                    Raycast(data, pointerClick);
                }

                data.Use();
            }
        }

        public void OnPointerExit(PointerEventData data) {
            tapCount = data.button == PointerEventData.InputButton.Left
                ? 0
                : Input.touchCount;

            if (ValidInput(eventData)) {
                curPosition = Vector2.zero;
                prevPosition = Vector2.zero;
                eventData = null;
                Raycast(data, pointerExit);
                data.Use();
            }
        }

        private void Raycast<T>(PointerEventData data,
                                ExecuteEvents.EventFunction<T> pointerEvent) where T : IEventSystemHandler {
            Vector3 start = ToWorldPos(data.position);
            float len = cam.nearClipPlane + cam.farClipPlane;
            int hits = Physics.RaycastNonAlloc(start,
                                               Vector3.forward,
                                               results,
                                               len,
                                               layerDefault);

            if (hits == 0) {
                return;
            }

            Array.Sort(results, hitComparer);

            RaycastHit hit = results[hits - 1];
            ExecuteEvents.Execute(hit.collider.gameObject,
                                  data,
                                  pointerEvent);
        }

        private Vector3 ToWorldPos(Vector2 screenPos) {
            return cam.ScreenToWorldPoint(new Vector3(screenPos.x,
                                                      screenPos.y,
                                                      cam.nearClipPlane));
        }

        public static ExecuteEvents.EventFunction<IPointerHoldHandler> pointerHold {
            get { return ExecutePointerHold; }
        }
        public static ExecuteEvents.EventFunction<IPointerClickHandler> pointerClick {
            get { return ExecutePointerClick; }
        }
        public static ExecuteEvents.EventFunction<IPointerUpHandler> pointerUp {
            get { return ExecutePointerUp; }
        }
        public static ExecuteEvents.EventFunction<IPointerDownHandler> pointerDown {
            get { return ExecutePointerDown; }
        }
        public static ExecuteEvents.EventFunction<IPointerExitHandler> pointerExit {
            get { return ExecutePointerExit; }
        }

        public static void ExecutePointerHold(IPointerHoldHandler   handler,
                                              BaseEventData         data) {
            handler.OnPointerHold(ExecuteEvents
                                  .ValidateEventData<PointerEventData>(data));
        }
        public static void ExecutePointerClick(IPointerClickHandler handler,
                                               BaseEventData        data) {
            handler.OnPointerClick(ExecuteEvents
                                   .ValidateEventData<PointerEventData>(data));
        }
        public static void ExecutePointerDown(IPointerDownHandler   handler,
                                              BaseEventData         data) {
            handler.OnPointerDown(ExecuteEvents
                                  .ValidateEventData<PointerEventData>(data));
        }
        public static void ExecutePointerUp(IPointerUpHandler   handler,
                                            BaseEventData       data) {
            handler.OnPointerUp(ExecuteEvents
                                .ValidateEventData<PointerEventData>(data));
        }
        public static void ExecutePointerExit(IPointerExitHandler   handler,
                                              BaseEventData         data) {
            handler.OnPointerExit(ExecuteEvents
                                  .ValidateEventData<PointerEventData>(data));
        }

    }

    public interface IPointerHoldHandler : IEventSystemHandler {
        void OnPointerHold(PointerEventData data);
    }
}
