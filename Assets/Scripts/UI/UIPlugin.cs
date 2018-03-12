using UnityEngine;
using UnityEngine.Events;
using System;
using Mandarin;
using Mandarin.PluginSystem;
using PaperPlaneTools;

namespace Syng {
    public class MOpenedPanel : IMessage {
        public UIPanelType panelType;
    }

    [Serializable]
    public class UIPanelTypeEvent : UnityEvent<UIPanelType> {}

    [RegisterPlugin]
    public class UIPlugin : MonoBehaviour, IPlugin {

        [SerializeField]
        private UIPanelTypeEvent    onSwitchedPanel;
        private UIPanel[]           panels;
        private int                 defaultPanel;
        private int                 curPanel;

		private int                 languageId;

		private Messenger msg;

        public void Init(Messenger msg) {
			this.msg = msg;

            defaultPanel = 0;
            panels = transform.GetComponentsInChildren<UIPanel>();

            for (int i=0; i<panels.Length; i++) {
                if (panels[i].panelType == UIPanelType.DEFAULT) {
                    defaultPanel = i;
                }
            }

            OpenPanel(UIPanelType.DEFAULT);

			msg.AddListener<MLanguageLoaded>(OnLanguageLoaded);
        }

        public void Ready(Messenger msg) {
        }

        public void OpenPanel(UIPanelType panel) {
            DeactivatePanel(curPanel);
            curPanel = FindPanel(panel);
            ActivatePanel(curPanel);
            onSwitchedPanel.Invoke(panel);
            msg.Dispatch(new MOpenedPanel { panelType = panel });
        }

        public void ClosePanel() {
            DeactivatePanel(curPanel);
            ActivatePanel(defaultPanel);
        }

		public void ShowRating() {
			msg.Dispatch(new MShowRatingPopup ());
		}

		public void OpenFacebook() {
			switch (languageId) {
			case 0:  Application.OpenURL("https://m.facebook.com/syngapp/"); break;
			default: Application.OpenURL("https://m.facebook.com/theSINGapp/"); break;
			}
		}

		public void OpenInstagram() {
			switch (languageId) {
			case 0:  Application.OpenURL("https://www.instagram.com/syng_app/"); break;
			default: Application.OpenURL("https://www.instagram.com/sing_app/"); break;
			}
		}

		private void OnLanguageLoaded(MLanguageLoaded languageLoaded) {
			this.languageId = languageLoaded.language;
		}

        private int FindPanel(UIPanelType type) {
            for (int i=0; i<panels.Length; i++) {
                if (panels[i].panelType == type) {
                    return i;
                }
            }
            return 0;
        }

        private void ActivatePanel(int panel) {
            panels[panel].gameObject.SetActive(true);
        }

        private void DeactivatePanel(int panel) {
            panels[panel].gameObject.SetActive(false);
        }
    }
}
