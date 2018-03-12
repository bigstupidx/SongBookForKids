using UnityEngine;
using System.Collections;
using Syng;
using Syng.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(SceneLoader))]
public class Preloader : MonoBehaviour, ISceneLoaded, IMainReady, IMainInit {

    public CanvasGroup     canvasGroup;
    public Camera          preloadCamera;
    public Camera          rtCamera;
    public Animator        animator;

    private CoroutineHandler fadeHandler;

    void Awake() {
        fadeHandler = new CoroutineHandler(this, FadeOut(2f, canvasGroup));
        GetComponent<SceneLoader>()
            .OnSceneLoaded(this, 1)
            .LoadAsync(1);
    }

    public void OnSceneLoaded(Scene scene) {
        preloadCamera.tag = "Untagged";

        GameObject[] rootObjects = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++) {
            Main main = rootObjects[i].GetComponent<Main>();
            if (main == null) {
                continue;
            }
            main.OnInit(this);
            break;
        }
    }

    public void OnMainReady() {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade() {
        fadeHandler.Start();
        yield return fadeHandler.coroutine;
        preloadCamera.enabled = false;
        rtCamera.enabled = false;
        animator.enabled = false;
    }

    private IEnumerator FadeOut(float sec, CanvasGroup canvasGroup) {
        yield return new WaitForSeconds(0.5f);
        float alpha = canvasGroup.alpha;
        while (alpha > 0f) {
            alpha -= Time.deltaTime / sec;
            canvasGroup.alpha = alpha;
            yield return null;
        }
    }

    public void OnMainInit(Main main) {
        main.OnReady(this);
        main.Init();
    }
}
