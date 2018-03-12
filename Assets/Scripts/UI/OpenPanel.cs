using UnityEngine;
using UnityEngine.Events;

namespace Syng {

    [System.Serializable]
    public class UIPanelEvent : UnityEvent<UIPanelType> {}

    public class OpenPanel : MonoBehaviour {

        public UIPanelType      panel;
        public UIPanelEvent     onOpenPanel;

        public void OnOpenPanel() {
            onOpenPanel.Invoke(panel);
        }

    }
}
