using UnityEngine;
using System.Collections.Generic;

namespace Mandarin.PluginSystem {
    public interface IPluginManagerData {
        List<IPlugin>   plugins { get; }
        Transform       mbPluginsContainer { get; set; }
    }
}
