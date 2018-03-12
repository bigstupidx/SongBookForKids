using UnityEngine;
using System.Collections;

namespace Syng {
    [RequireComponent(typeof(AudioSource))]
    public class BearBehaviour : MonoBehaviour {

        [Range(0f, 1f)]
        [SerializeField]
        private float               flicker;
        [SerializeField]
        private float               hotDogPlayTime;
        [SerializeField]
        private Gradient            colorRamp;
        [SerializeField]
        private BonfireBehaviour    bonfire;
        [SerializeField]
        private ParticleSystem      hotDogFire;
        [SerializeField]
        private MeshRenderer[]      glows;
        private float               intensity;
        private MaterialPropertyBlock[] mblocks;

        void Awake() {
            mblocks = new MaterialPropertyBlock[glows.Length];
            for (int i=0; i<glows.Length; i++) {
                mblocks[i] = new MaterialPropertyBlock();
            }
        }

        void Update() {
            intensity = bonfire.intensity;
            float offset = Random.value * flicker - (flicker * .5f);
            intensity += offset;

            for (int i=0; i<glows.Length; i++) {
                glows[i].GetPropertyBlock(mblocks[i]);
                mblocks[i].SetColor("_Color", colorRamp.Evaluate(intensity));
                glows[i].SetPropertyBlock(mblocks[i]);
            }
        }

        public void OnSing() {
            StartCoroutine(BurnHotDog());
        }

        IEnumerator BurnHotDog() {
            yield return new WaitForSeconds(hotDogPlayTime);
            hotDogFire.Play();
        }
    }
}
