using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AndroidBackButton : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
