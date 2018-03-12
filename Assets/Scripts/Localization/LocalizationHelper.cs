using System;
using UnityEngine;

namespace Syng {
    public static class LocalizationHelper {

        public static string DEFAULT_FILENAME =     "default_lang";
        public static string DEFAULT_FILENAME_EXT = "default_lang.txt";
        
        public static int GetLanguage() {
            if (!PlayerPrefs.HasKey(LocalizationPlugin.PREFS_KEY)) {
                
                int lang = GetLanguageValue(Application.systemLanguage);

                TextAsset defLang = Resources.Load<TextAsset>(DEFAULT_FILENAME);
                if (defLang != null) {
                    lang = Int32.Parse(defLang.text);
                }
                
                return lang;
            }
            
            // Get lang from player prefs, and default to norwegian if
            // for some reason the key is not found.
            return PlayerPrefs.GetInt(LocalizationPlugin.PREFS_KEY, 0);
        }

        public static int GetLanguageValue(SystemLanguage lang) {
            switch (lang) {
                case SystemLanguage.Norwegian:
                case SystemLanguage.Swedish:
                case SystemLanguage.Danish:
                case SystemLanguage.Icelandic:
                case SystemLanguage.Finnish:
                case SystemLanguage.Faroese:
                    return 0;
                default:
                    return 1;
            }
        }
    }
}
