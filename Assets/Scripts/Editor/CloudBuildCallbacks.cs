using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Copy paste to Unity Cloud Build
// Syng.CloudBuildCallbacks.PreExport

namespace Syng {
    static public class CloudBuildCallbacks {

        public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest) {
            PlayerSettings.bundleVersion = manifest.GetValue<string>("buildNumber");
            ParallaxHelper.Reindex();
        }
    }
}

#if (!UNITY_CLOUD_BUILD)
namespace UnityEngine.CloudBuild {
    public class BuildManifestObject : ScriptableObject {

        // Tries to get a manifest value - returns true if key was found and
        // could be cast to type T, false otherwise.
        public bool TryGetValue<T>(string key, out T result) {
            result = default(T);
            return true;
        }

        // Retrieve a manifest value or throw an exception if the given key
        // isn't found.
        public T GetValue<T>(string key) { return default(T); }

        // Sets the value for a given key.
        public void SetValue(string key, object value) {}

        // Copy values from a dictionary. ToString() will be called on
        // dictionary values before being stored.
        public void SetValues(Dictionary<string, object> sourceDict) {}

        // Remove all key/value pairs
        public void ClearValues() {}

        // Returns a Dictionary that represents the current BuildManifestObject
        public Dictionary<string, object> ToDictionary() { return null; }

        // Returns a JSON formatted string that represents the current
        // BuildManifestObject
        public string ToJson() { return ""; }

        // Returns an INI formatted string that represents the current
        // BuildManifestObject
        public override string ToString() { return ""; }
    }
}
#endif
