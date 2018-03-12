using UnityEngine;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

// TODO: Should be able to pass a config object to a plugin. Each plugin should
// have a set of defaults in case a config object is not passed. When and
// how to pass it?

// - all plugins are registered and inited at system startup, so config
//   needs to be available before that.
// ? how do we pass the right config object to the right plugin
// ! if a config object gets a propertydrawer/or not, they can all be put in
//   a list and exposed in the inspector. Could use a scriptable obj.
// ! PluginManData interface could include a slot for a master config object.
//   It should be PluginManager's responsibility to either pass the config
//   object to the plugin, or init a new one.


namespace Mandarin.PluginSystem {
    public class PluginManager {

        private IPluginManagerData  data;

        public Messenger            messenger { get; private set; }

        public PluginManager(IPluginManagerData data) {
            this.data = data;
            messenger = new Messenger();
        }

        public PluginManager LoadPlugins() {
            InstantiatePlugins(FindPlugins(""));
            return this;
        }

        public PluginManager LoadPlugins(string ns) {
            InstantiatePlugins(FindPlugins(ns));
            return this;
        }

        public PluginManager LoadPlugins(Type[] pluginTypes, string ns = "") {
            List<PluginData> plugs = FindPlugins(ns);
            GetPluginsFromParams(plugs, pluginTypes);
            InstantiatePlugins(plugs);
            return this;
        }

        public void Ready() {
            Ready(data.plugins);
        }

        private void InstantiatePlugins(List<PluginData> plugs) {
            foreach (PluginData plugin in plugs) {
                IPlugin instance = plugin.instance;

                if (typeof(MonoBehaviour).IsAssignableFrom(plugin.type)) {
                    if (instance == null) {
                        if (data.mbPluginsContainer == null) {
                            data.mbPluginsContainer = new GameObject("MonoBehaviourPlugins").transform;
                        }

                        GameObject mbInstance = new GameObject(plugin.type.Name);
                        mbInstance.transform.parent = data.mbPluginsContainer;
                        instance = mbInstance.AddComponent(plugin.type) as IPlugin;
                    }
                } else {
                    instance = (IPlugin)Activator.CreateInstance(plugin.type);
                }

                data.plugins.Add(instance);
            }

            Init(data.plugins);
        }

        private List<PluginData> FindPlugins(string ns) {
            List<PluginData> plugs = new List<PluginData>(50);
            GetPluginsFromScene(ns, plugs);
            GetPluginsFromAssembly(ns, plugs);
            return plugs;
        }

        private void Init(List<IPlugin> plugins) {
            for (int i=0, l=plugins.Count; i<l; i++) {
                plugins[i].Init(messenger);
            }
        }

        private void Ready(List<IPlugin> plugins) {
            for (int i=0, l=plugins.Count; i<l; i++) {
                plugins[i].Ready(messenger);
            }
        }

        private void GetPluginsFromParams(List<PluginData> list,
                                          Type[] types) {
            for (int i=0, l=types.Length; i<l; i++) {
                Type type = types[i];
                if (type.IsAbstract) {
                    continue;
                }
                object[] attributes = type.GetCustomAttributes(true);
                if (attributes.Length == 0) {
                    continue;
                }
                foreach (object attribute in attributes) {
                    var attrib = attribute as RegisterPluginAttribute;
                    if (attrib == null) {
                        continue;
                    }
                    if (Registered(list, type)) {
                        continue;
                    }
                    list.Add(new PluginData {
                        type        = type
                    });
                }
            }
        }

        #if UNITY_IOS
        [Conditional("__NEVER_DEFINED__")]
        #endif
        private void GetPluginsFromAssembly(string ns, List<PluginData> list) {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type type in assembly.GetTypes()) {
                if (type.IsAbstract) {
                    continue;
                }

                object[] attributes = type.GetCustomAttributes(true);
                if (attributes.Length == 0) {
                    continue;
                }

                foreach (object attribute in attributes) {
                    if (attribute is RegisterPluginAttribute) {
                        if (!typeof(IPlugin).IsAssignableFrom(type)) {
                            UnityEngine.Debug.LogError(
                                "Trying to register plugin " + type +
                                " but it doesn't implement the IPlugin "+
                                "interface");
                            continue;
                        }
                        if (Registered(list, type)) {
                            continue;
                        }
                        if (ContainsNamespace(type.Namespace, ns)) {
                            list.Add(new PluginData {
                                type        = type
                            });
                        }
                    }
                }
            }
        }

        private void GetPluginsFromScene(string ns, List<PluginData> list) {
            MonoBehaviour[] monoBehaviours = FindAll<MonoBehaviour>();
            List<IPlugin> monoPlugins = new List<IPlugin>(50);

            foreach (MonoBehaviour mb in monoBehaviours) {
                if (mb is IPlugin) {
                    monoPlugins.Add(mb as IPlugin);
                }
            }

            foreach (IPlugin plugin in monoPlugins) {
                Type type = plugin.GetType();
                object[] attributes = type.GetCustomAttributes(true);
                if (attributes.Length == 0) {
                    continue;
                }

                foreach (object attribute in attributes) {
                    if (attribute is RegisterPluginAttribute) {
                        if (ContainsNamespace(type.Namespace, ns)) {
                            list.Add(new PluginData {
                                type        = type,
                                instance    = plugin
                            });
                        }
                    }
                }
            }
        }

        private bool ContainsNamespace(string typeNS, string ns) {
            if (String.IsNullOrEmpty(ns)) {
                return true;
            }
            if (String.IsNullOrEmpty(typeNS)) {
                return String.IsNullOrEmpty(ns);
            }
            return typeNS.Substring(0, ns.Length) == ns;
        }

        private T[] FindAll<T>() where T : Component {
            return GameObject.FindObjectsOfType(typeof(T)) as T[];
        }

        private bool Registered(List<PluginData> data, Type type) {
            for (int i=0, l=data.Count; i<l; i++) {
                if (data[i].type == type) {
                    return true;
                }
            }
            return false;
        }
    }
}
