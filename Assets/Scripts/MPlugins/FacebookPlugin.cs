using UnityEngine;
using UnityEngine.Analytics;
using Mandarin;
using Mandarin.PluginSystem;
using System.Collections.Generic;
using Facebook.Unity;

namespace Syng {

    [RegisterPlugin]
    public class FacebookPlugin : MonoBehaviour, IPlugin {

        public void Init(Messenger msg) {}

        public void Ready(Messenger msg) {
            if (!FB.IsInitialized) {
                FB.Init(OnInit);
            }
        }

        private void OnInit() {
            if (!FB.IsInitialized) {
                Debug.Log("Failed to initialize Facebook SDK...");
            } else {
                FB.ActivateApp();
            }
        }

        public void OnApplicationPause(bool pauseStatus) {
            if (!pauseStatus) {
                if (FB.IsInitialized) {
                    FB.ActivateApp();
                } else {
                    FB.Init(OnInit);
                }
            }
        }
    }
}
