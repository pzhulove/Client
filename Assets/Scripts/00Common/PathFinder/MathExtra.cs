using System.Collections;

namespace PathFinder
{

    public static class MathExtra
    {

        public static float FastSqrt(float x)
        {
            unsafe
            {
                int i;
                float x2, y;
                const float threehalfs = 1.5F;
                x2 = x * 0.5F;
                y = x;
                i = *(int*)&y;
                i = 0x5f375a86 - (i >> 1);
                y = *(float*)&i;
                y = y * (threehalfs - (x2 * y * y));
                return x * y;
            }
        }
        public static float InverseSqrtFast(float x)
        {
            unsafe
            {
                float xhalf = 0.5f * x;
                int i = *(int*)&x;
                i = 0x5f375a86 - (i >> 1);
                x = *(float*)&i;
                x = x * (1.5f - xhalf * x * x);
                return x;
            }
        }

    }

}