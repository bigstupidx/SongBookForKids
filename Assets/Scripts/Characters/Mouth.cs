using UnityEngine;
using System;
using System.Collections.Generic;

public enum MouthState {
    OPEN = 0,
    CLOSED = 1,
    SMILE = 2,
    SURPRISED = 3
}

[Serializable]
public class MouthConfig {
    public MouthState   state;
    public Sprite       sprite;
}

[RequireComponent(typeof(SpriteRenderer))]
public class Mouth : MonoBehaviour {

    public MouthState               defaultState;
    public MouthConfig[]            configs;

    private Dictionary<int, int>    stateMap;
    private SpriteRenderer          sRenderer;

    void Awake() {
        stateMap = new Dictionary<int, int>(configs.Length);
        for (int i=0; i<configs.Length; i++) {
            int ci = -1;
            for (int j=0; i<configs.Length; j++) {
                if ((int)configs[j].state == i) {
                    ci = j;
                    break;
                }
            }
            stateMap.Add(i, ci);
        }
        sRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        SetMouth(defaultState);
    }

    private void SetMouth(MouthState state) {
        int ci = -1;
        if (stateMap.TryGetValue((int)state, out ci)) {
            sRenderer.sprite = configs[ci].sprite;
        }
    }

}

