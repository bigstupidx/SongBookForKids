namespace Mandarin {
    static public class M {

        static public float Sign(float value) {
            return value > 0f ? 1f : value < 0f ? -1f : 0f;
        }

        static public uint Abs(int n) {
            int mask = n >> 31;
            return (uint)((n ^ mask) - mask);
        }

        static public float Map(float val, float mina, float maxa, float minb, float maxb) {
            return minb + (maxb - minb) * ((val - mina) / (maxa - mina));
        }
    }
}
