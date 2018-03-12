using UnityEngine;
using UnityEngine.Events;

namespace Syng {
    public class GetUIPanel : MonoBehaviour {

        [SerializeField]
        private UIPanelType type;
        [SerializeField]
        private UnityEvent  gotPanel;

        public void OnGotPanel(UIPanelType panelType) {
            if (panelType != type) {
                return;
            }
            gotPanel.Invoke();
        }

    }
}
