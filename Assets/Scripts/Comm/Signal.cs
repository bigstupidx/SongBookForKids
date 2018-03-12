using System;
using System.Collections.Generic;

namespace Mandarin {

    // Shit class!

    public class Signal<T> where T : class {
        public List<T>      callbacks;
        private Action<T>   callback;

        public Signal() {
            callbacks = new List<T>();
        }

        public Signal<T> SetCallback(Action<T> cb) {
            callback = cb;
            return this;
        }

        public void Add(T listener) {
            callbacks.Remove(listener);
            callbacks.Add(listener);
        }

        public void Invoke() {
            for (int i=0, l=callbacks.Count; i<l; i++) {
                callback(callbacks[i]);
            }
        }

        public void Invoke(Action<T> callback) {
            for (int i=0, l=callbacks.Count; i<l; i++) {
                callback(callbacks[i]);
            }
        }

        public int handlers {
            get { return callbacks.Count; }
        }
    }
}
