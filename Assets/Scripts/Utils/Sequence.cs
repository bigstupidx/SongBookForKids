using UnityEngine;

namespace Syng {

    public enum PlayOrder {
        SEQUENTIAL = 0,
        RANDOM = 1,
        PATTERN = 2,
        BY_PARAMETER = 3,
    }

    public interface IPlayOrder {
        void Sort(int[] sequence);
    }

    public class PlayOrderSeq : IPlayOrder {
        public void Sort(int[] sequence) {
            for (int i=0; i<sequence.Length; i++) {
                sequence[i] = i;
            }
        }
    }

    public class PlayOrderRnd : IPlayOrder {
        public void Sort(int[] sequence) {
            // Randomize using Fisher-Yates algorithm
            for (int i=sequence.Length; i>1; i--) {
                int j = Random.Range(0, (i-1));
                int tmp = sequence[j];
                sequence[j] = sequence[i - 1];
                sequence[i - 1] = tmp;
            }
        }
    }

    public class PlayOrderBlank : IPlayOrder {
        public void Sort(int[] sequence) {}
    }

    public class Sequence {

        [SerializeField]
        private PlayOrder       playOrder;
        [SerializeField]
        private int[]           sequence;
        private IPlayOrder[]    orderers;
        private int             current;

        public Sequence(int length, PlayOrder order) {
            current = 0;
            playOrder = order;

            if ((int)playOrder != 2) {
                sequence = new int[length];
                // Fill the array, otherwise the randomization won't have anything
                // to randomize
                for (int i=0; i<sequence.Length; i++) {
                    sequence[i] = i;
                }
            }

            orderers = new IPlayOrder[] {
                new PlayOrderSeq(),
                new PlayOrderRnd(),
                new PlayOrderBlank(),
                new PlayOrderSeq()
            };

            GetOrderer(playOrder).Sort(sequence);
        }

        private IPlayOrder GetOrderer(PlayOrder po) {
            return orderers[(int)po];
        }

        public int GetNext() {
            // Store current index
            int i = current;
            // Increase the value to make it ready for the next call. If we
            // had simply increased the counter before returning it,
            // we would have to set the start value to -1 for it to start at 0
            current++;
            if (current == sequence.Length) {
                GetOrderer(playOrder).Sort(sequence);
                current = 0;
            }
            return i;
        }

    }
}
