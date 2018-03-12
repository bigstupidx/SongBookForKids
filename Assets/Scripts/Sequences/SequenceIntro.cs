using UnityEngine;
using System.Collections;
using Syng;
using UnityEngine.EventSystems;
using EventSystem = UnityEngine.EventSystems.EventSystem;

// start after delay
// wait until dandelion plays two notes
// pan to purple flower
// play purple flower
// play sequence on spider string
// wait
// play spider song

public class SequenceIntro : MonoBehaviour {

    public CameraRigPlugin rig;
    public TapHandler purpleFlower;
    public SpiderThread spiderThread;
    public TapHandler spider;

    private float delay = 7f;
    private float firstPan = 3f;
    private float flowerPlayTime = 2f;
    private float secondPan = 3.7f;
    private float panSpeed = 50f; // sec per total length
    private float thirdPan = 0.5f;
    private bool pan;

    private float threadTapArea;
    private float threadExtentY;
    private Camera camera;

    void Start() {
        Main.msg.AddListener<MGetCamera>(OnGetCamera);
        threadTapArea = spiderThread.GetComponent<BoxCollider>().bounds.size.y / 4f;
        threadExtentY = spiderThread.GetComponent<BoxCollider>().bounds.extents.y;
        StartCoroutine(Sequence());
    }

    private void OnGetCamera(MGetCamera msg) {
        camera = msg.camera;
    }

    IEnumerator Sequence() {
        yield return new WaitForSeconds(delay);
        pan = true;
        yield return new WaitForSeconds(firstPan);
        pan = false;
        purpleFlower.OnPointerClick(null);
        yield return new WaitForSeconds(flowerPlayTime);
        pan = true;
        yield return new WaitForSeconds(secondPan);
        pan = false;
        yield return new WaitForSeconds(thirdPan);
        spiderThread.Pluck(GetPED(GetPluckPos(0)));
        yield return new WaitForSeconds(0.4f);
        spiderThread.Pluck(GetPED(GetPluckPos(0)));
        yield return new WaitForSeconds(0.3f);
        spiderThread.Pluck(GetPED(GetPluckPos(1)));
        yield return new WaitForSeconds(0.5f);
        spiderThread.Pluck(GetPED(GetPluckPos(2)));
        yield return new WaitForSeconds(0.5f);
        spiderThread.Pluck(GetPED(GetPluckPos(3)));
        yield return new WaitForSeconds(1f);
        spider.OnPointerClick(null);
    }

    void Update() {
        if (!pan) {
            return;
        }

        rig.MoveCamera(Time.deltaTime / panSpeed);
    }

    private PointerEventData GetPED(Vector2 click) {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = click;
        return ped;
    }

    private Vector2 GetPluckPos(int i) {
        Vector3 halfArea = (threadTapArea / 2f) * Vector3.up;
        Vector3 offset = halfArea + threadTapArea * i * Vector3.up;
        Vector3 pos = spiderThread.transform.position - (threadExtentY * Vector3.up) + offset;
        return camera.WorldToScreenPoint(pos);
    }
}
