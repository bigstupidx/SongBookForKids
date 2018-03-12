using UnityEngine;
using Mandarin;
using Mandarin.PluginSystem;

namespace Syng {

    public class MUnloadPrefab : IMessage {
        public GameObject   prefab;
        public int          index;
    }

    public class MLoadedPrefab : IMessage {
        public GameObject   prefab;
        public int          index;
    }

    public class MUnloadAudio : IMessage {
        public AudioClip    clip;
        public int          index;
    }

    public class MLoadedAudio : IMessage {
        public AudioClip    clip;
        public int          index;
    }

    public class MLoadLanguage : IMessage {
        public int          language;
    }

    public class MLanguageAssetsLoaded : IMessage {
        public int          language;
    }

    public class MLanguageLoaded : IMessage {
        public int          language;
    }

    [RegisterPlugin]
    public class LocalizationPlugin : MonoBehaviour, IPlugin {

        static public string        PREFS_KEY = "SYNG_Language";

        [SerializeField]
        private string[]            languages;

        private GameObject[]        prefabs;
        [SerializeField]
        private string[]            prefabKeys;

        private AudioClip[]         audioClips;
        [SerializeField]
        private string[]            audioKeys;

        private MUnloadPrefab       mUnloadPrefab;
        private MLoadedPrefab       mLoadedPrefab;
        private MUnloadAudio        mUnloadAudio;
        private MLoadedAudio        mLoadedAudio;
        private MLanguageAssetsLoaded     mLanguageAssetsLoaded;
        private MLanguageLoaded     mLanguageLoaded;
        private Messenger           msg;
        private bool                isLoading;
        private ResourceRequest[]   prefabRequests;
        private ResourceRequest[]   audioRequests;
        private int                 loadedPrefabs;
        private int                 loadedAudioClips;
        private int                 curLang;

        public void Init(Messenger msg) {
            this.msg = msg;
            curLang = -1;
            mUnloadPrefab = new MUnloadPrefab();
            mLoadedPrefab = new MLoadedPrefab();
            mUnloadAudio = new MUnloadAudio();
            mLoadedAudio = new MLoadedAudio();
            mLanguageAssetsLoaded = new MLanguageAssetsLoaded();
            mLanguageLoaded = new MLanguageLoaded();
            prefabs = new GameObject[prefabKeys.Length];
            audioClips = new AudioClip[audioKeys.Length];
            prefabRequests = new ResourceRequest[prefabKeys.Length];
            audioRequests = new ResourceRequest[audioKeys.Length];

            msg.AddListener<MLoadLanguage>(OnLoadLanguage);
        }

        public void Ready(Messenger msg) {
        }

        void Update() {
            if (!isLoading) {
                return;
            }

            ResourceRequest rr;
            for (int i=0; i<prefabRequests.Length; i++) {
                rr = prefabRequests[i];
                if (rr.isDone && prefabs[i] == null) {
                    prefabs[i] = GameObject.Instantiate(rr.asset as GameObject);
                    prefabs[i].SetActive(false);
                    prefabs[i].transform.parent = transform;

                    mLoadedPrefab.prefab = prefabs[i];
                    mLoadedPrefab.index = i;
                    msg.Dispatch(mLoadedPrefab);

                    loadedPrefabs++;
                }
            }

            for (int i=0; i<audioRequests.Length; i++) {
                rr = audioRequests[i];
                if (rr.isDone && audioClips[i] == null) {
                    audioClips[i] = rr.asset as AudioClip;

                    mLoadedAudio.clip = audioClips[i];
                    mLoadedAudio.index = i;
                    msg.Dispatch(mLoadedAudio);

                    loadedAudioClips++;
                }
            }

            bool donePrefabs = loadedPrefabs == prefabRequests.Length;
            bool doneAudio = loadedAudioClips == audioRequests.Length;

            if (donePrefabs && doneAudio) {
                mLanguageAssetsLoaded.language = curLang;
                msg.Dispatch(mLanguageAssetsLoaded);
                mLanguageLoaded.language = curLang;
                msg.Dispatch(mLanguageLoaded);
                isLoading = false;
            }
        }

        public string[] GetLanguages() {
            return languages;
        }

        public string[] GetPrefabKeys() {
            return prefabKeys;
        }

        public string[] GetAudioKeys() {
            return audioKeys;
        }

        public void LoadLanguage(int lang) {
            if (isLoading) {
                return;
            }

            if (lang == curLang) {
                return;
            }

            curLang = lang;
            PlayerPrefs.SetInt(PREFS_KEY, lang);
            loadedPrefabs = 0;
            loadedAudioClips = 0;

            // Unload any loaded assets
            for (int i=0; i<prefabs.Length; i++) {
                if (prefabs[i] == null) {
                    continue;
                }
                mUnloadPrefab.prefab = prefabs[i];
                mUnloadPrefab.index = i;
                msg.Dispatch(mUnloadPrefab);

                GameObject.Destroy(prefabs[i]);
                prefabs[i] = null;
            }

            for (int i=0; i<audioClips.Length; i++) {
                if (audioClips[i] == null) {
                    continue;
                }
                mUnloadAudio.clip = audioClips[i];
                mUnloadAudio.index = i;
                msg.Dispatch(mUnloadAudio);

                audioClips[i] = null;
            }

            Resources.UnloadUnusedAssets();

            string language = languages[lang];

            // Start the loading processes
            for (int i=0; i<prefabKeys.Length; i++) {
                prefabRequests[i] = LoadAsync<GameObject>(language + "/" + prefabKeys[i]);
            }

            for (int i=0; i<audioKeys.Length; i++) {
                audioRequests[i] = LoadAsync<AudioClip>(language + "/" + audioKeys[i]);
            }

            isLoading = true;
        }

        private void OnLoadLanguage(MLoadLanguage load) {
            LoadLanguage(load.language);
        }

        private ResourceRequest LoadAsync<T>(string path) where T : Object {
            return Resources.LoadAsync(path, typeof(T));
        }
    }
}
