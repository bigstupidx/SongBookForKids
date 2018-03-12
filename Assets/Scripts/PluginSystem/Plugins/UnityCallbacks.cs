using UnityEngine;
using Mandarin;
using Mandarin.PluginSystem;

namespace Mandarin.Plugins {

    // Messages

    public class MAddLateUpdateHandler : IMessage {
        public IUCLateUpdate handler;
        public MAddLateUpdateHandler(IUCLateUpdate handler) {
            this.handler = handler;
        }
    }

    public class MAddUpdateHandler : IMessage {
        public IUCUpdate handler;
        public MAddUpdateHandler(IUCUpdate handler) {
            this.handler = handler;
        }
    }

    // Interfaces

    public interface IUCUpdate {
        void UCUpdate();
    }

    public interface IUCLateUpdate {
        void UCLateUpdate();
    }

    // Callback handlers

    public class UCUpdate : MonoBehaviour {

        public Signal<IUCUpdate> CbUpdate;

        void Awake() {
            CbUpdate = new Signal<IUCUpdate>()
                .SetCallback(u => { u.UCUpdate(); });
        }

        void Update() {
            CbUpdate.Invoke();
        }
    }

    public class UCLateUpdate : MonoBehaviour {

        public Signal<IUCLateUpdate> CbLateUpdate;

        void Awake() {
            CbLateUpdate = new Signal<IUCLateUpdate>()
                .SetCallback(u => { u.UCLateUpdate(); });
        }

        void Update() {
            CbLateUpdate.Invoke();
        }
    }

    [RegisterPlugin]
    public class UnityCallbacks : MonoBehaviour, IPlugin {

        // TODO: Add support for coroutines

        private UCUpdate        ucUpdate;
        private UCLateUpdate    ucLateUpdate;

        public void Init(Messenger msg) {
            msg.AddListener<MAddUpdateHandler>(OnAddUpdate);
            msg.AddListener<MAddLateUpdateHandler>(OnAddLateUpdate);
        }

        public void Ready(Messenger msg) {}

        private void OnAddUpdate(MAddUpdateHandler msg) {
            if (ucUpdate == null) {
                ucUpdate = gameObject.AddComponent<UCUpdate>();
            }
            ucUpdate.CbUpdate.Add(msg.handler);
        }

        private void OnAddLateUpdate(MAddLateUpdateHandler msg) {
            if (ucLateUpdate == null) {
                ucLateUpdate = gameObject.AddComponent<UCLateUpdate>();
            }
            ucLateUpdate.CbLateUpdate.Add(msg.handler);
        }

    }
}
