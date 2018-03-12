using UnityEngine;
using UnityEngine.EventSystems;

public class Tappable : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick(PointerEventData data) {
        D.Log("Click", gameObject.name);
    }
}
