using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DebugLog : MonoBehaviour {

    public string message;

    public void Log(PointerEventData data) {
        Debug.Log(message);
    }
}
