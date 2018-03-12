using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syng {

    [CustomEditor(typeof(TextureImportConfig))]
    public class TextureImportConfigInspector : Editor {

        private StringBuilder               sb;
        private TIPlatformOverrides         platformOverrides;

        private string[]                    spriteModeLabels = new string[] {
            SpriteImportMode.Single.ToString(),
            SpriteImportMode.Multiple.ToString(),
            SpriteImportMode.Polygon.ToString()
        };

        private string[]                    importerLabels = new string[] {
            "Texture",
            "Normal map",
            "Editor GUI and Legacy GUI",
            "Cubemap",
            // Reflection
            "Cookie",
            "Advanced",
            "Lightmap",
            "Cursor",
            "Sprite (2D and UI)",
        };

        private int[]                       importerLabelsMap = new int[] {
            0, 1, 2, 3, -1, /* Reflection */ 4, 5, 6, 7, 8,
        };

        void OnEnable() {
            platformOverrides = new TIPlatformOverrides();
            sb = new StringBuilder();
        }

        void OnDisable() {
            // Show notification if dirty
        }

        public override void OnInspectorGUI() {
            int i = ImportTypeDropdown();
            GetInspector(i)(i);

            EditorGUILayout.Space();
            platformOverrides.DrawInspector(serializedObject,
                                            GetImportType());
            EditorGUILayout.Space();

            ApplyButton();

            EditorGUILayout.HelpBox("Apply will update the import settings "+
                                    "on all textures in all sub folders of "+
                                    "this config file. Sub folders that has "+
                                    "its own TextureImportConfig object will "+
                                    "be ignored.\n\nThe files will be "+
                                    "reimported if there are any differences "+
                                    "between the file's import settings and "+
                                    "the ones you set here.\n\nFiles that are "+
                                    "imported to this folder, or any sub "+
                                    "folder that hasn't got its own "+
                                    "TextureImportConfig object, will be "+
                                    "imported using the settings set here.",
                                    MessageType.Info);

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void ApplyButton() {
            if (GUILayout.Button("Apply")) {
                List<string> foundFiles = new List<string>();
                string path = TextureImportConfigHelpers.GetPath(target);
                TextureImportConfigHelpers.GetFilesInFolder(target, path, ref foundFiles);

                for (int i=0, len=foundFiles.Count; i<len; i++) {
                    string file = foundFiles[i];
                    EditorUtility.DisplayProgressBar("Reimporting asset "
                                                     + i + "/" + len,
                                                     file,
                                                     (float)i / (float)len);
                    AssetDatabase.ImportAsset(file);
                }

                EditorUtility.ClearProgressBar();
            }
        }

        private TextureImporterType GetImportType() {
            SerializedProperty type = serializedObject.FindProperty("textureType");
            string val = type.enumDisplayNames[type.enumValueIndex];
            return (TextureImporterType)Enum.Parse(typeof(TextureImporterType), val);
        }

        private int ImportTypeDropdown() {
            SerializedProperty type = serializedObject.FindProperty("textureType");
            int selected = importerLabelsMap[type.enumValueIndex];
            int newSelect = EditorGUILayout.Popup("Texture Type", selected, importerLabels);
            if (newSelect != selected) {
                selected = newSelect;
                if (newSelect > 3) {
                    newSelect += 1;
                }
                type.enumValueIndex = newSelect;
                serializedObject.ApplyModifiedProperties();
            }
            return selected;
        }

        private void DrawTextureSettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);
        }

        private void DrawNormalmapSettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);
        }

        private void DrawGUISettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);
        }

        private void DrawCubemapSettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);
        }

        private void DrawCookieSettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);
        }

        private void DrawAdvancedSettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);

            // Skipped:
            // - Non power of two
            // - Mapping

            // Add:
            // Readwrite
            // import type
            // alpha from grayscale
            // alpha is transp
            // bypass srgb
            // encode as rgbm
            // sprite mode!!
                // packing tag
                // ppu
                // mesh type
                // extude edges
            // mip maps
            // wrap mode
            // filter mode
            // aniso
        }

        private void DrawLightmapSettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);
        }

        private void DrawCursorSettings(int n) {
            EditorGUILayout.LabelField(importerLabels[n]);
        }

        private void DrawSpriteSettings(int n) {
            SerializedProperty importMode = serializedObject.FindProperty("spriteImportMode");
            if (importMode.enumValueIndex == (int)SpriteImportMode.None) {
                importMode.enumValueIndex = (int)SpriteImportMode.Single;
            }

            int modeIndex = EditorGUILayout.Popup("Sprite Mode", importMode.enumValueIndex - 1, spriteModeLabels);
            importMode.enumValueIndex = modeIndex + 1;

            EditorGUI.indentLevel++;
            {
                TextField("spritePackingTag", "Packing Tag");
                FloatField("spritePixelsPerUnit", "Pixels Per Unit");
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            ToggleField("mipmapEnabled", "Generate Mip Maps");
            EditorGUILayout.Space();

            EnumPopup("filterMode", "Filter Mode");
        }

        private void EnumPopup(string prop, string label) {
            SerializedProperty sp = serializedObject.FindProperty(prop);
            sp.enumValueIndex = EditorGUILayout.Popup(label, sp.enumValueIndex, sp.enumDisplayNames);
        }

        private void ToggleField(string prop, string label) {
            SerializedProperty sp = serializedObject.FindProperty(prop);
            sp.boolValue = EditorGUILayout.Toggle(label, sp.boolValue);
        }

        private void TextField(string prop, string label) {
            SerializedProperty sp = serializedObject.FindProperty(prop);
            sp.stringValue = EditorGUILayout.TextField(label, sp.stringValue);
        }

        private void FloatField(string prop, string label) {
            SerializedProperty sp = serializedObject.FindProperty(prop);
            sp.floatValue = EditorGUILayout.FloatField(label, sp.floatValue);
        }

        private Texture2D MakeTex(int width,
                                  int height,
                                  Color textureColor,
                                  RectOffset border,
                                  Color bordercolor) {
            int widthInner = width;
            width += border.left;
            width += border.right;
            int totalHeight = height + border.top + border.bottom;

            Color[] pix = new Color[ width * totalHeight];

            for (int i = 0; i < pix.Length; i++) {
                if (i < (border.bottom * width)) {
                    pix[i] = bordercolor;
                }
                //Border Top
                else if (i >= ((border.bottom * width) + (height * width))) {
                    pix[i] = bordercolor;
                }
                // Center of Texture
                else {
                    // Border left
                    if ((i%width) < border.left) {
                        pix[i] = bordercolor;
                    }
                    //Border right
                    else if ((i % width) >= (border.left + widthInner)) {
                        pix[i] = bordercolor;
                    }
                    //Color texture
                    else {
                        pix[i] = textureColor;
                    }
                }
            }

            Texture2D result = new Texture2D(width, totalHeight);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private string PascalCase(string str) {
            string[] parts = str.Split('_');
            sb.Remove(0, sb.Length);
            for (int i=0; i<parts.Length; i++) {
                sb.Append(parts[i][0]);
                sb.Append(parts[i].Substring(1).ToLower());
            }
            return sb.ToString();
        }

        private Action<int> GetInspector(int i) {
            Action<int>[] inspectors = new Action<int>[] {
                DrawTextureSettings,
                DrawNormalmapSettings,
                DrawGUISettings,
                DrawCubemapSettings,
                DrawCookieSettings,
                DrawAdvancedSettings,
                DrawLightmapSettings,
                DrawCursorSettings,
                DrawSpriteSettings,
            };
            return inspectors[i];
        }

    }
}
