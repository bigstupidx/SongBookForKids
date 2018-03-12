using UnityEditor;
using System.IO;

namespace Syng {
    public class TexturePostprocessor : AssetPostprocessor {

        private void OnPreprocessTexture() {
            TextureImportConfig tic;
            if (FindConfigIn(assetPath, out tic)) {
                tic.ApplyConfig(assetImporter);
            }
        }

        private bool FindConfigIn(string path, out TextureImportConfig tic) {
            string[] dirs = path.Split('/');
            // Start at length - 1 to skip the filename, and stop at
            // index 1 to avoid going beyond the Assets directory.
            for (int i=dirs.Length - 1; i>=1; i--) {
                string dir = Join(dirs, "/", i);
                string[] files = Directory.GetFiles(dir, "*.asset");
                for (int f=0; f<files.Length; f++) {
                    tic = Load<TextureImportConfig>(files[f]);
                    if (tic != null) {
                        return true;
                    }
                }
            }
            tic = null;
            return false;
        }

        private T Load<T>(string path) where T : class {
            return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }

        private string Join(string[] arr, string delimeter, int len) {
            string str = "";
            for (int i=0; i<len; i++) {
                str += arr[i] + delimeter;
            }
            return str;
        }
    }
}
