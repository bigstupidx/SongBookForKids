using UnityEngine;
using UnityEngine.Events;

namespace Syng {

    [System.Serializable]
    public class IntegerEvent : UnityEvent<int> {}
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> {}
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> {}
    [System.Serializable]
    public class Vector3Event : UnityEvent<Vector3> {}
    [System.Serializable]
    public class SpriteRendererEvent : UnityEvent<SpriteRenderer> {}
    [System.Serializable]
    public class BoundsEvent : UnityEvent<Bounds> {}
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> {}
    [System.Serializable]
    public class AudioClipEvent : UnityEvent<AudioClip> {}
    [System.Serializable]
    public class AnimatorEvent : UnityEvent<Animator> {}

}
