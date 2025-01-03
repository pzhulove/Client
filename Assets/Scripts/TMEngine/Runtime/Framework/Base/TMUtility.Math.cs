

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Math
        {
            private const float CONST_PI = 3.1415926535897932384626f;
            private const float CONST_ONE_OVER_PI = 1.0f / 3.1415926535897932384626f;
            private const float CONST_ONE_OVER_180 = 1.0f / 180.0f;

            static public float Clamp(float value,float min, float max)
            {
                //Debugger.Assert(min <= max, "Minimum must less than maximum!");
                if(min > max)
                {
                    float temp = min;
                    min = max;
                    max = temp;
                }

                float res = value < min ? min : value;
                res = res > max ? max : res;
                return res;
            }

            static public int Clamp(int value, int min, int max)
            {
                //Debugger.Assert(min <= max, "Minimum must less than maximum!");
                if(min > max)
                {
                    min ^= max;
                    max ^= min;
                    min ^= max;
                }

                int res = value < min ? min : value;
                res = res > max ? max : res;
                return res;
            }

            static public float DegreeToRadian(float degree)
            {
                return degree * CONST_ONE_OVER_180 * CONST_PI;
            }

            static public float RadianToDegree(float radian)
            {
                return radian * CONST_ONE_OVER_PI * 180f;
            }

            static public float Max(float a,float b)
            {
                return a > b ? a : b;
            }

            static public float Min(float a, float b)
            {
                return a < b ? a : b;
            }

            static public int Max(int a, int b)
            {
                return a > b ? a : b;
            }

            static public int Min(int a, int b)
            {
                return a < b ? a : b;
            }

            static public uint Min(uint a, uint b)
            {
                return a < b ? a : b;
            }

            static public uint Max(uint a, uint b)
            {
                return a > b ? a : b;
            }

            static public float Abs(float real)
            {
                return System.Math.Abs(real);
            }

            static public float Sqrt(float real)
            {
                return (float)System.Math.Sqrt(real);
            }

            static public double Sqrt(double real)
            {
                return System.Math.Sqrt(real);
            }

            static public double Sin(double radian)
            {
                return System.Math.Sin(radian);
            }

            static public float Sin(float radian)
            {
                return (float)System.Math.Sin(radian);
            }

            static public double Cos(double radian)
            {
                return System.Math.Cos(radian);
            }

            static public float Cos(float radian)
            {
                return (float)System.Math.Cos(radian);
            }

            static public double Tan(double radian)
            {
                return System.Math.Tan(radian);
            }

            static public float Tan(float radian)
            {
                return (float)System.Math.Tan(radian);
            }

            static public float Lerp(float a,float b,float factorAToB)
            {
                return a * (1 - factorAToB) + b * factorAToB;
            }
        }
    }
}