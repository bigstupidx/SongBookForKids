using UnityEditor;
using UnityEngine;
using System;

namespace Syng {

    public enum MaxTextureSize {
        POW5 = 32,
        POW6 = 64,
        POW7 = 128,
        POW8 = 256,
        POW9 = 512,
        POW10 = 1024,
        POW11 = 2048,
        POW12 = 4096,
        POW13 = 8192,
    }

    public class TIPlatformOverrides {

        private int         selected;
        private string[]    texFormats = new string[] {
            "Crunched", "Truecolor", "16 bits", "Compressed"
        };
        private string[]    tabLabels = new string[] {
            "PC, Mac & Linux Standalone", "iOS", "Android"
        };
        private string[]    overrideID = new string[] {
            "std_", "ios_", "and_"
        };
        private string[]    qualityLabels = new string[] {
            "Fast", "Normal", "Best"
        };
        private string[]    platformNames = new string[] {
            "Web", "Standalone", "iPhone", "Android"
        };
        // defaultMaxSize refers to the index of the size value in the
        // MaxTextureSize enum
        private int         defaultMaxSize = 5;

        static private TIPlatformOverrides self;

        public TIPlatformOverrides() {
            self = this;
            selected = 0;
        }

        public void DrawInspector(SerializedObject      so,
                                  TextureImporterType   importType) {
            EditorGUILayout.BeginHorizontal();
            for (int i=0; i<4; i++) {
                bool s = GUILayout.Toggle(i == selected,
                                          GetCaptionFor(i),
                                          GetStyleFor(i));
                selected = s ? i : selected;
            }
            EditorGUILayout.EndHorizontal();

            GUIStyle backgroundBox = new GUIStyle("box");
            backgroundBox.margin = new RectOffset(0, 0, 0, 0);
            EditorGUILayout.BeginVertical(backgroundBox);
            {
                string texSize = "maxTextureSize";
                string texFormat = "textureFormat";
                string alphaSplit = "allowsAlphaSplit";
                string compQual = "compressionQuality";

                if (selected > 0) {
                    GUI.enabled = ToggleOverride(so, selected - 1);
                    string id = overrideID[selected - 1];
                    texSize = id + texSize;
                    texFormat = id + texFormat;
                    compQual = id + compQual;
                }

                bool android = selected == 3;
                bool ios = selected == 2;

                SerializedProperty maxSize = so.FindProperty(texSize);
                MaxTextureSize mts = MaxTextureSize.POW5;
                int maxSel = GetIndex(mts, maxSize.intValue);
                maxSel = maxSel < 0 ? defaultMaxSize : maxSel;
                int maxSelNew = EditorGUILayout.Popup("Max Size", maxSel, GetLabels(mts));
                maxSize.intValue = GetValue(mts, maxSelNew);

                SerializedProperty frm = so.FindProperty(texFormat);
                int frmSel = frm.enumValueIndex < 0 ? 0 : frm.enumValueIndex;
                frm.enumValueIndex = EditorGUILayout.Popup("Format", frmSel, texFormats);

                bool crunched = TexFormatIs(frm, TextureImporterFormat.AutomaticCrunched);
                bool compressed = TexFormatIs(frm, TextureImporterFormat.AutomaticCompressed);

                // Crunched is not supported on iOS
                if (ios && crunched) {
                    EditorGUILayout.HelpBox("Crunched mode is not supported on "+
                                            "iOS. Unity will default to Compressed "+
                                            "when Crunch is selected.", MessageType.Warning);
                }

                SerializedProperty quality = so.FindProperty(compQual);

                // Show slider when:
                // Format = Crunched and Platform = default or standalone
                bool defaultOrStandalone = selected == 0 || selected == 1;
                if (crunched && defaultOrStandalone) {
                    quality.intValue = (int)EditorGUILayout.Slider("Compression Quality", quality.intValue, 0, 100);
                }

                // Show dropdown when:
                // Format = Crunched or Compressed and Platform = iOS or Android
                if ((crunched || compressed) && (ios || android)) {
                    int qi = (quality.intValue / 100) * 2;
                    qi = EditorGUILayout.Popup("Compression Quality", qi, qualityLabels);
                    quality.intValue = (qi / 2) * 100;
                }

                // Split alpha channel = false on all, except
                // when import type is anything but Advanced and override platform is Android
                SerializedProperty alpha = so.FindProperty(alphaSplit);
                if (importType != TextureImporterType.Default && android) {
                    alpha.boolValue = EditorGUILayout.ToggleLeft("Compress using ETC1 (split alpha channel)",
                                                                 alpha.boolValue);
                }

                GUI.enabled = true;
            }
            EditorGUILayout.EndVertical();
        }

        private bool TexFormatIs(SerializedProperty prop, TextureImporterFormat format) {
            string name = prop.enumNames[prop.enumValueIndex];
            return EnumParse<TextureImporterFormat>(name) == format;
        }

        // TODO: Extension method on Enum?
        private T EnumParse<T>(string name) {
            return (T)Enum.Parse(typeof(T), name);
        }

        private bool ToggleOverride(SerializedObject so, int i) {
            string label = "Override for " + tabLabels[i];
            string propName = overrideID[i] + "override";
            SerializedProperty over = so.FindProperty(propName);
            over.boolValue = EditorGUILayout.ToggleLeft(label, over.boolValue);
            return over.boolValue;
        }

        private int GetValue(Enum e, int index) {
            int[] sizes = Enum.GetValues(e.GetType()) as int[];
            return sizes[index];
        }

        private string[] GetLabels(Enum e) {
            int[] values = Enum.GetValues(e.GetType()) as int[];
            string[] labels = new string[values.Length];
            for (int i=0; i<values.Length; i++) {
                labels[i] = values[i].ToString();
            }
            return labels;
        }

        private int GetIndex(Enum e, int i) {
            int[] values = Enum.GetValues(e.GetType()) as int[];
            int j = -1;
            for (int n=0; n<values.Length; n++) {
                if (values[n] == i) {
                    j = n;
                }
            }
            return j;
        }

        private GUIStyle GetStyleFor(int i) {
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.stretchWidth = i == 0;
            if (i > 0) {
                style.fixedWidth = 32;
            }
            return style;
        }

        private GUIContent GetCaptionFor(int i) {
            GUIContent[] captions = new GUIContent[] {
                new GUIContent("Default"),
                Icon("Standalone"),
                Icon("iPhone"),
                Icon("Android"),
            };
            return captions[i];
        }

        private GUIContent Icon(string name) {
            string iconName = "d_BuildSettings." + name + ".Small";
            return new GUIContent(EditorGUIUtility.FindTexture(iconName));
        }

        static public int numPlatforms {
            get { return self.platformNames.Length; }
        }

        static public string GetPlatformName(int n) {
            return self.platformNames[n];
        }
    }
}
