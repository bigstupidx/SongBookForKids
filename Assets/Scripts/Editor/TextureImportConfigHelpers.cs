using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Syng {
    static public class TextureImportConfigHelpers {

        static private string[] validFileExtensions = new string[] {
            ".png", ".gif", ".jpg", ".jpeg"
        };

        static public string GetPath(Object asset) {
            return Application.dataPath.Replace("Assets", "") + GetAssetFolder(asset);
        }

        static private string GetAssetFolder(Object asset) {
            string folder = "";
            string[] parts = AssetDatabase.GetAssetPath(asset).Split('/');
            for (int i=0; i<parts.Length-1; i++) {
                folder += parts[i] + "/";
            }
            return folder;
        }

        static private string GetRelativePath(string absolutePath) {
            return absolutePath.Replace(Application.dataPath.Replace("Assets", ""), "");
        }

        static public void GetFilesInFolder(Object              target,
                                            string              folderPath,
                                            ref List<string>    foundFiles) {
            // Check if current folder has a config file
            string[] assets = Directory.GetFiles(folderPath, "*.asset");
            bool hasConfig = false;
            foreach (string asset in assets) {
                string path = GetRelativePath(asset);
                TextureImportConfig tic = LoadAssetAtPath<TextureImportConfig>(path);
                // TextureImportConfig tic = (TextureImportConfig)AssetDatabase.LoadAssetAtPath(GetRelativePath(asset), typeof(TextureImportConfig));
                if (tic != null && tic != target) {
                    hasConfig = true;
                    break;
                }
            }

            if (!hasConfig) {
                // Get list of all textures
                string [] files = Directory.GetFiles(folderPath);
                foreach (string file in files) {
                    if (IsValidFile(file)) {
                        foundFiles.Add(GetRelativePath(file));
                    }
                }
            }

            // Get list of all sub directories
            string[] subdirs = Directory.GetDirectories(folderPath);
            foreach (string subdir in subdirs) {
                GetFilesInFolder(target, subdir, ref foundFiles);
            }
        }

        static private T LoadAssetAtPath<T>(string path) where T : Object {
            return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
        }

        static public bool IsValidFile(string filePath) {
            string ext = Path.GetExtension(filePath);
            for (int i=0; i<validFileExtensions.Length; i++) {
                if (validFileExtensions[i] == ext) {
                    return true;
                }
            }
            return false;
        }
    }
}
