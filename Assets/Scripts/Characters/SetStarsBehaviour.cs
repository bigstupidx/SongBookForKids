using UnityEngine;

namespace Syng {
    public class SetStarsBehaviour : MonoBehaviour {

        [SerializeField]
        private StarsBehaviour  stars;

        public void SetStars(GameObject go) {
            go.GetComponent<MoonBehaviour>().SetStars(stars);
        }

    }
}
