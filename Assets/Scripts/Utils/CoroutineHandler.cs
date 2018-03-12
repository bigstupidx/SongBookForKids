using UnityEngine;
using System.Collections;

public class CoroutineHandler {

    public Coroutine      coroutine;
    public object         result;

    private IEnumerator   target;
    private MonoBehaviour owner;

    public CoroutineHandler(MonoBehaviour owner, IEnumerator enumerator) {
        target = enumerator;
        this.owner = owner;
    }

    public void Start() {
        coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run() {
        while (target.MoveNext()) {
            result = target.Current;
            yield return result;
        }
    }
}