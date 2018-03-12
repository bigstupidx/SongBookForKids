using UnityEngine;
using UnityEngine.Events;
using System;

namespace Syng {

    [Serializable]
    public class LocalizedPrefabEvent : UnityEvent<GameObject, int> {}
    [Serializable]
    public class LocalizedAudioEvent : UnityEvent<AudioClip, int> {}

    public class OnLocalizedAssets : MonoBehaviour {

        [SerializeField]
        private LocalizedPrefabEvent    onUnloadPrefab;
        [SerializeField]
        private LocalizedPrefabEvent    onLoadedPrefab;
        [SerializeField]
        private LocalizedAudioEvent     onUnloadAudio;
        [SerializeField]
        private LocalizedAudioEvent     onLoadedAudio;

        void Awake() {
            Main.msg
                .AddListener<MUnloadAudio>(OnUnloadAudio)
                .AddListener<MLoadedAudio>(OnLoadedAudio)
                .AddListener<MUnloadPrefab>(OnUnloadPrefab)
                .AddListener<MLoadedPrefab>(OnLoadedPrefab);
        }

        private void OnUnloadAudio(MUnloadAudio unload) {
            onUnloadAudio.Invoke(unload.clip, unload.index);
        }

        private void OnLoadedAudio(MLoadedAudio loaded) {
            onLoadedAudio.Invoke(loaded.clip, loaded.index);
        }

        private void OnUnloadPrefab(MUnloadPrefab unload) {
            onUnloadPrefab.Invoke(unload.prefab, unload.index);
        }

        private void OnLoadedPrefab(MLoadedPrefab loaded) {
            onLoadedPrefab.Invoke(loaded.prefab, loaded.index);
        }

    }
}
