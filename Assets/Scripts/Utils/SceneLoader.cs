using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Syng.Utils {

    public interface ISceneLoaded {
        void OnSceneLoaded(Scene scene);
    }

    public class SceneLoader : MonoBehaviour {

        private class LoadOperation {
            public AsyncOperation   operation;
            public int              sceneID;
        }

        private List<LoadOperation> loaders;
        private ISceneLoaded[]      handlers;
        private int                 numLoaders;

        void Awake() {
            loaders = new List<LoadOperation>();
            // Hardcoded number of scenes added to Build Settings
            handlers = new ISceneLoaded[2];
            numLoaders = 0;
        }

        public SceneLoader OnSceneLoaded(ISceneLoaded handler, int sceneID) {
            if (sceneID >= handlers.Length) {
                return this;
            }
            handlers[sceneID] = handler;
            return this;
        }

        // In this version, SceneLoader doesn't care when the scene
        // gets loaded. All scenes have a SceneLoaded component
        // at the root, which dispatches a message when loading
        // is complete.
        public SceneLoader LoadAsync(int           sceneID,
                                     LoadSceneMode mode = LoadSceneMode.Additive) {

            if (SceneManager.sceneCount > sceneID) {
                Scene scene = SceneManager.GetSceneAt(sceneID);
                if (!scene.IsValid()) {
                    return this;
                }
                if (scene.isLoaded) {
                    LoadDone(handlers[sceneID], sceneID);
                    return this;
                }
            }

            LoadOperation lo = new LoadOperation {
                sceneID = sceneID,
                operation = SceneManager.LoadSceneAsync(sceneID, mode)
            };
            loaders.Add(lo);
            numLoaders++;
            return this;
        }

        void Update() {
            for (int i=numLoaders-1; i>=0; i--) {
                if (!loaders[i].operation.isDone) {
                    continue;
                }

                --numLoaders;
                int sceneID = loaders[i].sceneID;
                LoadDone(handlers[sceneID], sceneID);
                enabled = numLoaders > 0;
                loaders.RemoveAt(i);
            }
        }

        private void LoadDone(ISceneLoaded handler, int sceneID) {
            if (handler == null) {
                return;
            }
            handler.OnSceneLoaded(SceneManager.GetSceneAt(sceneID));
        }
    }
}