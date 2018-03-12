using UnityEngine;
using UnityEditor;
using System;

namespace Syng {

    [CreateAssetMenu(fileName = "TextureImportConfig",
                     menuName = "Syng/Texture Import Config",
                     order = 1)]
    [Serializable]
    public class TextureImportConfig : ScriptableObject {

        [SerializeField]
        private TextureImporterType     textureType;

        [SerializeField]
        private SpriteImportMode        spriteImportMode;
        [SerializeField]
        private string                  spritePackingTag;
        [SerializeField]
        private float                   spritePixelsPerUnit;

        [SerializeField]
        private bool                    mipmapEnabled;

        [SerializeField]
        private FilterMode              filterMode;

        [SerializeField]
        private int                     maxTextureSize;
        [SerializeField]
        private int                     std_maxTextureSize;
        [SerializeField]
        private int                     ios_maxTextureSize;
        [SerializeField]
        private int                     and_maxTextureSize;

        [SerializeField]
        private TextureImporterFormat   textureFormat;
        [SerializeField]
        private TextureImporterFormat   std_textureFormat;
        [SerializeField]
        private TextureImporterFormat   ios_textureFormat;
        [SerializeField]
        private TextureImporterFormat   and_textureFormat;

        [SerializeField]
        private bool                    allowsAlphaSplit;

        [SerializeField]
        private int                     compressionQuality;
        [SerializeField]
        private int                     std_compressionQuality;
        [SerializeField]
        private int                     ios_compressionQuality;
        [SerializeField]
        private int                     and_compressionQuality;

        [SerializeField]
        private bool                    std_override;
        [SerializeField]
        private bool                    ios_override;
        [SerializeField]
        private bool                    and_override;

        public void ApplyConfig(AssetImporter assetImporter) {
            TextureImporter ti = (TextureImporter)assetImporter;

            ti.textureType = textureType;
            ti.spriteImportMode = spriteImportMode;
            ti.spritePackingTag = spritePackingTag;
            ti.spritePixelsPerUnit = spritePixelsPerUnit;
            ti.mipmapEnabled = mipmapEnabled;
            ti.filterMode = filterMode;
            ti.maxTextureSize = maxTextureSize;
            ti.textureFormat = textureFormat;
            ti.compressionQuality = compressionQuality;

            if (std_override) {
                ti.SetPlatformTextureSettings("Standalone",
                                              std_maxTextureSize,
                                              std_textureFormat,
                                              std_compressionQuality,
                                              false);
            } else {
                ti.ClearPlatformTextureSettings("Standalone");
            }

            if (ios_override) {
                ti.SetPlatformTextureSettings("iPhone",
                                              ios_maxTextureSize,
                                              ios_textureFormat,
                                              ios_compressionQuality,
                                              false);
            } else {
                ti.ClearPlatformTextureSettings("iPhone");
            }

            if (and_override) {
                ti.SetAllowsAlphaSplitting(allowsAlphaSplit);
                ti.SetPlatformTextureSettings("Android",
                                              and_maxTextureSize,
                                              and_textureFormat,
                                              and_compressionQuality,
                                              allowsAlphaSplit);
            } else {
                ti.ClearPlatformTextureSettings("Android");
                ti.SetAllowsAlphaSplitting(false);
            }
        }
    }
}
