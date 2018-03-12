using UnityEngine;
using System.Collections;
using Syng;

public class SequenceSheep : MonoBehaviour {

    public CameraRigPlugin rig;
    public TapHandler dandelion;
    public TapHandler elderFlower;
    public Transform fox;

    private float camStartPos = -0.106f;
    private float panSpeed = 50f;
    private float delay = 4f;
    private float firstPan = 5f;
    private float secondPan = 2.5f;
    private float thirdPan = 2f;

    private bool pan;
    private TapHandler foxTap;

    void Awake() {
    }

    void Start() {
        Main.msg.AddListener<MLanguageLoaded>(OnLangLoaded);
        rig.MoveCamera(camStartPos + 0.5f);
        StartCoroutine(Sequence());
    }

    private void OnLangLoaded(MLanguageLoaded msg) {
        foxTap = fox.GetComponentInChildren<TapHandler>();
    }

    IEnumerator Sequence() {
        yield return new WaitForSeconds(delay);
        pan = true;
        yield return new WaitForSeconds(firstPan);
        pan = false;
        dandelion.OnPointerClick(null);
        yield return new WaitForSeconds(0.3f);
        dandelion.OnPointerClick(null);
        yield return new WaitForSeconds(0.3f);
        dandelion.OnPointerClick(null);
        yield return new WaitForSeconds(0.3f);
        pan = true;
        yield return new WaitForSeconds(secondPan);
        pan = false;
        elderFlower.OnPointerClick(null);
        yield return new WaitForSeconds(1.1f);
        pan = true;
        yield return new WaitForSeconds(thirdPan);
        pan = false;
        foxTap.OnPointerClick(null);
    }

    void Update() {
        if (!pan) {
            return;
        }
        rig.MoveCamera(Time.deltaTime / panSpeed);
    }
}
