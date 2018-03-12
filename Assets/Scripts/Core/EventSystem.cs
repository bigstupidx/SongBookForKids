using UnityEngine;

namespace Syng
{
    public class EventSystem : UnityEngine.EventSystems.EventSystem
    {
        [Range(0, 30)]
        [Tooltip("Drag Threshold for Android only")]
        [SerializeField]
        private int dragThresholdAndroid = 7;

        public int DragThresholdAndroid
        {
            get { return dragThresholdAndroid; }
            set { dragThresholdAndroid = value; }
        }

        private const float OneDPIScreen = 160;

        [Range(0, 30)]
        [Tooltip("Drag Threshold for Windows Store Apps only")]
        [SerializeField]
        private int dragThresholdWindows = 25;

        protected override void OnEnable()
        {
            UpdateDragTreshold();
            base.OnEnable();
        }

        protected override void Update()
        {
            UpdateDragTreshold();
            base.Update();
        }

        private void UpdateDragTreshold()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    //dragThresholdAndroid = Mathf.RoundToInt((value * 100) / Screen.dpi);
                    pixelDragThreshold = Mathf.Max(dragThresholdAndroid, Mathf.RoundToInt(dragThresholdAndroid * Screen.dpi / OneDPIScreen));
                    break;
                case RuntimePlatform.WSAPlayerARM:
                    pixelDragThreshold = dragThresholdWindows;
                    break;
            }
        }

        public static bool IsBackButtonDown()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
    }
}