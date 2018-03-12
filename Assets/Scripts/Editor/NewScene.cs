using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

static public class NewScene {

    [MenuItem("File/New Syng Scene %&n")]
    static public void CreateNewScene() {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            return;
        }

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,
                                                  NewSceneMode.Single);

        Instantiate("Prefabs/Main", scene);
        Instantiate("Prefabs/CameraRig", scene);

        RenderSettings.ambientIntensity = 1f;
        RenderSettings.ambientLight = Color.white;

        EditorSceneManager.MarkSceneDirty(scene);
    }

    static private void Instantiate(string asset, Scene scene) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/"+asset+".prefab");
        PrefabUtility.InstantiatePrefab(prefab, scene);
    }
}




