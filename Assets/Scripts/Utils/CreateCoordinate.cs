using UnityEngine;
using UnityEngine.Events;
using System;

namespace Syng {

    [System.Serializable]
    public class UnityEventV3 : UnityEvent<Vector3> {}

    public enum CreateCoordinateMode {
        FIXED = 0,
        RANDOM_UNIT = 1,
        RANDOM_BOUND_RECT = 2,
        // RANDOM_BOUND_CIRCLE = 2,
    }

    public class CreateCoordinate : MonoBehaviour {

        [SerializeField]
        private CreateCoordinateMode    mode;

        // Fixed
        [SerializeField]
        private Vector3                 fixedCoordinate;

        // Random unit Vector3
        [SerializeField]
        private Vector3                 unitAxis;

        // Random withing square boundaries
        [SerializeField]
        private Rect                    boundaries;
        [SerializeField]
        private bool                    boundariesWorldSpace;

        [SerializeField]
        private UnityEventV3            onCoordCreated;

        private Func<Vector3>[]         createMethods;

        void Awake() {
            createMethods = new Func<Vector3>[] {
                CreateFixed,
                CreateRandomUnit,
                CreateRandomSquare
            };
        }

        private Vector3 CreateFixed() {
            return fixedCoordinate;
        }

        private Vector3 CreateRandomUnit() {
            return new Vector3(Rnd * unitAxis.x,
                               Rnd * unitAxis.y,
                               Rnd * unitAxis.z);
        }

        private Vector3 CreateRandomSquare() {
            Rect b = ToScreenSpace(boundaries);
            return new Vector3(Rnd * b.width + b.x,
                               Rnd * b.height + b.y,
                               0f);
        }

        private Rect ToScreenSpace(Rect rect) {
            rect.y = rect.y - rect.height;
            return rect;
        }

        private Func<Vector3> GetCreateMethod() {
            return createMethods[(int)mode];
        }

        private float Rnd {
            get { return UnityEngine.Random.value; }
        }

        public void OnCreateCoordinate() {
            Func<Vector3> Create = GetCreateMethod();
            onCoordCreated.Invoke(Create());
        }
    }
}
