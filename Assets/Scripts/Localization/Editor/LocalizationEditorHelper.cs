using UnityEngine;
using System.IO;
using Syng;

public static class LocalizationEditorHelper {

    public static void WriteDefaultLang(SystemLanguage lang) {
        string path = Application.dataPath + "/Resources/" + LocalizationHelper.DEFAULT_FILENAME_EXT;
        File.WriteAllText(path, LocalizationHelper.GetLanguageValue(lang).ToString());
    }
}
