using UnityEngine;
using UnityEngine.Analytics;
using Mandarin;
using Mandarin.PluginSystem;
using System.Collections.Generic;

namespace Syng {

    public class MAnalyticSong : IMessage {
        public string songName;
    }

    [RegisterPlugin]
    public class AnalyticsPlugin : MonoBehaviour, IPlugin {

        private string[] langs = new string[] {"NO", "ENG"};
        private int curLang = 0;

        public void Init(Messenger msg) {
            msg
                .AddListener<MAnalyticSong>(OnLogSong)
                .AddListener<MLoadLanguage>(OnLoadedLang)
                .AddListener<MOpenedPanel>(OnOpenedPanel);
        }

        public void Ready(Messenger msg) {}

        private void OnLogSong(MAnalyticSong song) {
            Analytics.CustomEvent("playSong", new Dictionary<string, object> {
                { "song", song.songName },
                { "lang", langs[curLang] }
            });
        }

        private void OnLoadedLang(MLoadLanguage lang) {
            curLang = lang.language;
        }

        private void OnOpenedPanel(MOpenedPanel openedPanel) {
            Analytics.CustomEvent("openPanel", new Dictionary<string, object> {
                { "panelType", openedPanel.panelType.ToString() },
            });
        }
    }
}
