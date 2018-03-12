using System;
using System.Collections.Generic;

namespace Mandarin {

    public interface IMessage {
    }

    public delegate void MessageDelegate (IMessage msg);
    public delegate void MessageDelegate<T> (T msg) where T : IMessage;

    public class Messenger {

        private Dictionary<Type, MessageDelegate> delegates;
        private Dictionary<Delegate, MessageDelegate> delegateLookup;

        public Messenger() {
            delegates = new Dictionary<Type, MessageDelegate>();
            delegateLookup = new Dictionary<Delegate, MessageDelegate>();
        }

        static public Messenger Create() {
            return new Messenger();
        }

        public Messenger AddListener<T> (MessageDelegate<T> del, Predicate<T> validator = null) where T : IMessage {
            // Early-out if we've already registered this delegate
            if (delegateLookup.ContainsKey(del)) {
                return this;
            }

            // Create a new non-generic delegate which calls our generic one.
            // This is the delegate we actually invoke.
            MessageDelegate mdel;
            if (validator == null) {
                mdel = msg => {
                    del((T)msg);
                };
            } else {
                mdel = msg => {
                    if (validator((T)msg)) {
                        del((T)msg);
                    }
                };
                delegateLookup[del] = mdel;
            }

            MessageDelegate tempDel;
            delegates[typeof(T)] = delegates.TryGetValue(typeof(T), out tempDel)
                ? tempDel += mdel
                : mdel;

            return this;
        }

        public Messenger RemoveListener<T> (MessageDelegate<T> del) where T : IMessage {
            MessageDelegate internalDelegate;
            if (delegateLookup.TryGetValue(del, out internalDelegate)) {

                MessageDelegate tempDel;
                if (delegates.TryGetValue(typeof(T), out tempDel)) {
                    tempDel -= internalDelegate;
                    if (tempDel == null) {
                        delegates.Remove(typeof(T));
                    } else {
                        delegates[typeof(T)] = tempDel;
                    }
                }

                delegateLookup.Remove(del);
            }

            return this;
        }

        public Messenger Dispatch(IMessage msg) {
            MessageDelegate del;
            if (delegates.TryGetValue(msg.GetType(), out del)) {
                del.Invoke(msg);
            }
            return this;
        }
    }
}
