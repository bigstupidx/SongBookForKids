using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LogTapScreenPos : MonoBehaviour {

    void Awake() {
    }

    void Start() {
    }

    public void OnClick(PointerEventData data) {
        Debug.Log(data.position);
    }
}
