using System;
using System.Collections;
using UnityEngine;
using Mandarin;
using Mandarin.Plugins;
using Mandarin.PluginSystem;

namespace Syng {

    public interface IMainReady {
        void OnMainReady();
    }

    public interface IMainInit {
        void OnMainInit(Main main);
    }

    [AddComponentMenu("Syng/Main")]
    public class Main : MonoBehaviour {

        static public Messenger msg;

        private IMainReady readyHandler;
        private IMainInit initHandler;
        private PluginManager pman;

        void Awake() {
            Application.targetFrameRate = 30;

            Type[] plugins = new Type[] {
                typeof(LocalizationPlugin),
                typeof(CameraRigPlugin),
                typeof(AmbientPlugin),
                typeof(AnalyticsPlugin),
				typeof(FacebookPlugin),
				typeof(RatingPlugin),
                typeof(AudioMasterPlugin),
                typeof(UnityCallbacks)
            };

            pman = new PluginManager(new PluginManagerData());
            msg = pman.LoadPlugins(plugins).messenger;

            msg.AddListener<MLanguageLoaded>(OnLanguageLoaded);
        }

        void Start() {
            StartCoroutine(InitCallback());
//            Init();
        }

        private IEnumerator InitCallback() {
            yield return new WaitForEndOfFrame();
            if (initHandler != null) {
                initHandler.OnMainInit(this);
            }
        }

        public void OnInit(IMainInit handler) {
            initHandler = handler;
        }

        public void OnReady(IMainReady handler) {
            readyHandler = handler;
        }

        private void OnLanguageLoaded(MLanguageLoaded mLanguageLoaded) {
            if (readyHandler == null) {
                return;
            }
            readyHandler.OnMainReady();
        }

        public void Init() {
            pman.Ready();

            msg.Dispatch(new MLoadLanguage {
                language = LocalizationHelper.GetLanguage()
            });
        }
    }
}
