namespace Tenmove.Runtime.Math {
    public enum Axis {
        X,
        Y,
        Z,
    }

    public enum AxisMask {
        None = 0x00,
        X = 0x01,
        Y = 0x02,
        Z = 0x04,
        XY = X | Y,
        YZ = Y | Z,
        ZX = Z | X,
        XYZ = X | Y | Z,
    }

    public enum Direction {
        Forward,
        Backward,
        Upward,
        Downward,
        Leftward,
        Rightward,
    }

    public enum Plane {
        XY,
        YZ,
        ZX,
    }

    public enum Limit {
        Min,
        Max,
    }

    public static class TMath {

        public const float PI = 3.14159274F;

        public static int Abs (int f) {
            return System.Math.Abs (f);
        }
        public static float Abs (float f) {
            return System.Math.Abs (f);
        }

        public static int Clamp (int value, int min, int max) {
            return value<min?min : value> max?max : value;
        }

        public static float Clamp (float value, float min, float max) {
            return value<min?min : value> max?max : value;
        }

        public static float Sqrt (float value) {
            return (float) System.Math.Sqrt (value);
        }

        public static double Sqrt (double value) {
            return System.Math.Sqrt (value);
        }

        public static float Lerp (float a, float b, float t) {
            return a + (b - a) * t;
        }

        public static int Min (int l, int r) {
            return l < r?l : r;
        }

        public static float Min (float l, float r) {
            return l < r?l : r;
        }

        public static double Min (double l, double r) {
            return l < r?l : r;
        }

        public static int Max (int l, int r) {
            return l > r?l : r;
        }

        public static float Max (float l, float r) {
            return l > r?l : r;
        }

        public static double Max (double l, double r) {
            return l > r?l : r;
        }

        public static float Cos (float angle) {
            return (float) System.Math.Cos (angle);
        }

        public static double Cos (double angle) {
            return System.Math.Cos (angle);
        }

        public static float Sin (float angle) {
            return (float) System.Math.Sin (angle);
        }

        public static double Sin (double angle) {
            return System.Math.Sin (angle);
        }

        public static float Tan (float angle) {
            return (float) System.Math.Tan (angle);
        }

        public static double Tan (double angle) {
            return System.Math.Tan (angle);
        }
    }
}