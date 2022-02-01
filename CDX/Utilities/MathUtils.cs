namespace CDX.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static  class MathUtils
    {
        public static float Length(float x, float y)
        {
            return MathF.Sqrt(x * x + y * y);
        }

        public static float Clamp(float v, float min, float max)
        {
            if (v > max) v = max;
            if (v < min) v = min;
            return v;
        }

        //public static int NumberOfLeadingZeros(long i)
        //{
        //    // HD, Figure 5-6
        //    if (i == 0)                return 64;
        //    int n = 1;
        //    int x = (int)(i >> 32);
        //    if (x == 0) { n += 32; x = (int)i; }
        //    if (x >> 16 == 0) { n += 16; x <<= 16; }
        //    if (x >> 24 == 0) { n += 8; x <<= 8; }
        //    if (x >> 28 == 0) { n += 4; x <<= 4; }
        //    if (x >> 30 == 0) { n += 2; x <<= 2; }
        //    n -= x >> 31;
        //    return n;
        //}
        //public static int NextPowerOfTwo(int value)
        //{
        //    if (value == 0) return 1;
        //    value--;
        //    value |= value >> 1;
        //    value |= value >> 2;
        //    value |= value >> 4;
        //    value |= value >> 8;
        //    value |= value >> 16;
        //    return value + 1;
        //}
    }
}
