using UnityEngine;
using System.Collections.Generic;
using Mandarin.PluginSystem;

namespace Syng {

    public class PluginManagerData : IPluginManagerData {
        private List<IPlugin>   _plugins;

        public List<IPlugin> plugins {
            get { return _plugins; }
        }
        public Transform mbPluginsContainer { get; set; }

        public PluginManagerData() {
            int size = 25;
            _plugins = new List<IPlugin>(size);
        }
    }
}
