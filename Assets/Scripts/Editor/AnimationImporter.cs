using UnityEditor;
using System;

public class AnimationImporter : AssetPostprocessor {

    public void OnPreprocessModel() {
        if (!assetPath.StartsWith("Assets/Animations/", StringComparison.Ordinal)) {
            return;
        }
        ModelImporter mi = assetImporter as ModelImporter;
        mi.importMaterials = false;
        mi.globalScale = 100f;
        mi.isReadable = false;
    }

}
