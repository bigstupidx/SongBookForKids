using UnityEngine;
using System.Collections;

public class RunInBackground : MonoBehaviour {

    void Awake() {
        Application.runInBackground = true;
    }
}
