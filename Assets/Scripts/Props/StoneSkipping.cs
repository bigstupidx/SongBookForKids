using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mandarin;

namespace Syng {

    public class StoneSkipAnim {
        public SkippingStone    stone;
        public SkippingSplash[] splashes;
        public int              numSkips;
        public Vector3[]        skipCoords;
        public float            curPlayTime;
        public int              curCoord;
    }

    public class StoneSkipping : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [SerializeField]
        private SkippingSplash              prefabSplash;
        [SerializeField]
        private SkippingStone               prefabStone;
        [SerializeField]
        private Transform                   throwingStonesParent;

        [SerializeField]
        private float                       maxThrowDistance = 500f;
        [SerializeField]
        private float                       maxHorizontalAngle = 70f;
        [SerializeField]
        [RangeAttribute(1,10)]
        private int                         numSkipsPerThrow = 6;
        [SerializeField]
        private float                       firstSkipOffset = 200f;
        [SerializeField]
        private AnimationCurve              skipDistribution;
        [SerializeField]
        private float                       skipAnimDuration = 3f;

        // float skipHeight
        // float skipSpeed

        private GOPool                      stonePool;
        private GOPool                      splashPool;
        private ObjectPool<StoneSkipAnim>   anims;
        private float[]                     playTimes;

        void Awake() {
            stonePool = GOPool.Create(25)
                .Grow(true)
                .Parent(GO.Create("StonePool"))
                .Fill(prefabStone.gameObject);

            splashPool = GOPool.Create(50)
                .Grow(true)
                .Parent(GO.Create("SplashPool"))
                .Fill(prefabSplash.gameObject)
                .SetOnSpawn(splash => {
                    splash.GetComponent<SkippingSplash>()
                        .onAnimDone.AddListener(OnSplashAnimDone);
                });

            anims = ObjectPool<StoneSkipAnim>.Create(20)
                .Grow(false)
                .Fill(i => {
                    return new StoneSkipAnim {
                        skipCoords = new Vector3[numSkipsPerThrow],
                        splashes = new SkippingSplash[numSkipsPerThrow]
                    };
                })
                .SetOnDespawn(OnDespawnAnim);

            playTimes = new float[numSkipsPerThrow];
            // Calculate duration time per frame
            CalcPlayTimes(numSkipsPerThrow, ref playTimes);

            // get child objects from throwingStones. Pool them and refill
            // with as many as there are child objects.
        }

        void Update() {
            for (int i=0; i<anims.actives.Count; i++) {
                Play(anims.actives[i]);
            }
        }

        private void OnSplashAnimDone(SkippingSplash splash) {
            splash.onAnimDone.RemoveListener(OnSplashAnimDone);
            splashPool.Despawn(splash.transform);
        }

        private void OnDespawnAnim(StoneSkipAnim anim) {
            stonePool.Despawn(anim.stone.transform);
        }

        private void MockAnim() {
            float angle = Random.value * (maxHorizontalAngle * 2f) - maxHorizontalAngle;
            float force = Mathf.Clamp(Random.value, 0.2f, 1f);
            CreateAnim(angle, force);
        }

        private void Play(StoneSkipAnim anim) {
            anim.curPlayTime -= Time.deltaTime;

            if (anim.curPlayTime > 0f) {
                return;
            }

            anim.stone.transform.position = anim.skipCoords[anim.curCoord] + transform.position;
            anim.stone.Play();

            anim.splashes[anim.curCoord].Play();
            anim.curCoord++;
            if (anim.curCoord >= anim.numSkips) {
                anims.Despawn(anim);
                return;
            }

            anim.curPlayTime = playTimes[anim.curCoord];
        }

        private void PreloadSplashes(StoneSkipAnim anim) {
            ClearSplashes(ref anim.splashes);

            for (int i=0; i<anim.numSkips; i++) {
                Transform splash;
                splashPool.Spawn(out splash);
                anim.splashes[i] = splash.GetComponent<SkippingSplash>();

                // TODO: Make clamping customizable?
                float scale = 1f - Mathf.Clamp(GetSkipPosAtIndex(i), 0f, 0.9f);

                GO.Modify(splash)
                    .SetScale(new Vector3(scale, scale, 0f))
                    .SetPosition(transform.position + anim.skipCoords[i]);
            }
        }

        private void ClearSplashes(ref SkippingSplash[] splashes) {
            for (int i=0; i<splashes.Length; i++) {
                splashes[i] = null;
            }
        }

        private float GetSkipPosAtIndex(int index) {
            float skipLen = throwDist / numSkipsPerThrow;
            return skipDistribution.Evaluate((skipLen * index) / throwDist);
        }

        private void CreateAnim(float angle, float force) {
            StoneSkipAnim anim;
            if (anims.Spawn(out anim)) {
                anim.numSkips = (int)Mathf.Round(force * numSkipsPerThrow);

                // Skip coordinates
                ClearCoords(ref anim.skipCoords);
                CreateSkipCoords(angle, anim.numSkips, ref anim.skipCoords);

                PreloadSplashes(anim);

                anim.curCoord = 0;
                // Setting curPlayTime to 0f will make the first splash
                // render immediately. Add a value when animating the stone
                anim.curPlayTime = 0f;

                Transform stone;
                stonePool.Spawn(out stone);
                anim.stone = stone.GetComponent<SkippingStone>();
            }
        }

        private void CalcPlayTimes(int frames, ref float[] times) {
            float total = 0f;
            for (int i=1, j=0; i<=frames; i++, j++) {
                float dur = GetSkipPosAtIndex(i) * skipAnimDuration;
                float durMod = dur;
                if (j > 0) {
                    durMod -= total;
                }
                times[j] = durMod;
                total = dur;
            }
        }

        private void CreateSkipCoords(float           angle,
                                      int             numSkips,
                                      ref Vector3[]   coords) {
            for (int i=0; i<numSkips; i++) {
                float skipPosY = GetSkipPosAtIndex(i);
                float adjacent = skipPosY * throwDist + firstSkipOffset;
                float opposite = Mathf.Tan(angle * Mathf.Deg2Rad) * adjacent;
                coords[i] = new Vector3(opposite, adjacent, 0f);
            }
        }

        private void ClearCoords(ref Vector3[] coords) {
            for (int i=0; i<coords.Length; i++) {
                coords[i] = Vector3.zero;
            }
        }

        public void OnPointerDown(PointerEventData data) {
            // Log coordinate
        }

        public void OnPointerUp(PointerEventData data) {
            // MockAnim();
            #if UNITY_EDITOR
            CalcPlayTimes(numSkipsPerThrow, ref playTimes);
            #endif
            CreateAnim(25f, 1f);
        }

        public float throwDist {
            get { return maxThrowDistance - firstSkipOffset; }
        }

        public List<StoneSkipAnim> animations {
            get { return anims == null ? null : anims.actives; }
        }

    }
}
