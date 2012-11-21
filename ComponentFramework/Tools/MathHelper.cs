using System;
using SlimDX;

namespace ComponentFramework.Tools
{
    public static class MathHelper
    {
        public const float Pi = (float)Math.PI;
        public const float HalfPi = (float)(Math.PI / 2);
        public const float TwoPi = (float)(Math.PI * 2);

        public static bool AlmostEqual(double a, double b, double epsilon)
        {
            return Math.Abs(a - b) <= epsilon;
        }
        public static bool AlmostEqual(double a, double b)
        {
            return Math.Abs(a - b) <= float.Epsilon;
        }

        public static bool AlmostEqual(Vector3 a, Vector3 b, double epsilon)
        {
            return AlmostEqual(a.X, b.X, epsilon) && AlmostEqual(a.Y, b.Y, epsilon) && AlmostEqual(a.Z, b.Z, epsilon);
        }
        public static bool AlmostEqual(Vector3 a, Vector3 b)
        {
            return AlmostEqual(a.X, b.X) && AlmostEqual(a.Y, b.Y) && AlmostEqual(a.Z, b.Z);
        }

        public static float AlmostClamp(double value, double center, double epsilon)
        {
            return (float)(AlmostEqual(value, center, epsilon) ? center : value);
        }
        public static float AlmostClamp(double value, double center)
        {
            return (float) (AlmostEqual(value, center) ? center : value);
        }

        public static Vector3 AlmostClamp(Vector3 vector, double center, double epsilon)
        {
            return new Vector3(AlmostClamp(vector.X, center, epsilon),
                               AlmostClamp(vector.Y, center, epsilon),
                               AlmostClamp(vector.Z, center, epsilon));
        }
        public static Vector3 AlmostClamp(Vector3 vector, double center)
        {
            return new Vector3(AlmostClamp(vector.X, center),
                               AlmostClamp(vector.Y, center),
                               AlmostClamp(vector.Z, center));
        }

        public static float WrapAngle(double angle)
        {
            angle = (float)Math.IEEERemainder(angle, TwoPi);
            if (angle <= -Pi)
                return (float) (angle + TwoPi);
            if (angle > Pi)
                return (float) (angle - TwoPi);
            return (float) angle;
        }

        public static float Clamp(double value, double minimum, double maximum)
        {
            return (float) Math.Max(Math.Min(value, maximum), minimum);
        }
        public static int Clamp(int value, int minimum, int maximum)
        {
            return Math.Max(Math.Min(value, maximum), minimum);
        }

        public static float Lerp(double from, double to, double step)
        {
            return (float) ((to - from) * step + from);
        }

        public static float Max(params float[] items)
        {
            if (items.Length == 1) return items[0];
            var max = Math.Max(items[0], items[1]);
            for (int i = 1; i < items.Length; i++)
                max = Math.Max(max, items[i]);
            return max;
        }
        public static float Min(params float[] items)
        {
            if (items.Length == 1) return items[0];
            var min = Math.Min(items[0], items[1]);
            for (int i = 1; i < items.Length; i++)
                min = Math.Min(min, items[i]);
            return min;
        }

        public static float Max(params int[] items)
        {
            if (items.Length == 1) return items[0];
            var max = Math.Max(items[0], items[1]);
            for (int i = 1; i < items.Length; i++)
                max = Math.Max(max, items[i]);
            return max;
        }
        public static float Min(params int[] items)
        {
            if (items.Length == 1) return items[0];
            var min = Math.Min(items[0], items[1]);
            for (int i = 1; i < items.Length; i++)
                min = Math.Min(min, items[i]);
            return min;
        }

        public static float Saturate(float value)
        {
            return value < 0 ? 0 : value > 1 ? 1 : value;
        }
        public static float Saturate(double value)
        {
            return (float)(value < 0 ? 0 : value > 1 ? 1 : value);
        }

        public static float Frac(double number)
        {
            return (float)(number - (int)number);
        }

        public static int AsNumeric(this bool b)
        {
            return b ? 1 : 0;
        }

        public static Vector2 Sign(Vector2 vector)
        {
            return new Vector2(Math.Sign(vector.X), Math.Sign(vector.Y));
        }
        public static Vector3 Sign(Vector3 vector)
        {
            return new Vector3(Math.Sign(vector.X), Math.Sign(vector.Y), Math.Sign(vector.Z));
        }

        public static Vector2 Abs(Vector2 vector)
        {
            return new Vector2(Math.Abs(vector.X), Math.Abs(vector.Y));
        }
        public static Vector3 Abs(Vector3 vector)
        {
            return new Vector3(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }

        public static int Round(double number)
        {
            return (int)Math.Round(number, MidpointRounding.AwayFromZero);
        }

        public static Vector2 Round(Vector2 vector)
        {
            return new Vector2((float)Math.Round(vector.X, MidpointRounding.AwayFromZero), (float)Math.Round(vector.Y, MidpointRounding.AwayFromZero));
        }
        public static Vector3 Round(Vector3 vector)
        {
            return new Vector3((float)Math.Round(vector.X, MidpointRounding.AwayFromZero), (float)Math.Round(vector.Y, MidpointRounding.AwayFromZero), (float)Math.Round(vector.Z, MidpointRounding.AwayFromZero));
        }

        public static Vector2 Floor(Vector2 vector)
        {
            return new Vector2((float)Math.Floor(vector.X), (float)Math.Floor(vector.Y));
        }
        public static Vector3 Floor(Vector3 vector)
        {
            return new Vector3((float)Math.Floor(vector.X), (float)Math.Floor(vector.Y), (float)Math.Floor(vector.Z));
        }

        public static float ToDegrees(double radians)
        {
            return (float)(radians * 180 / Pi);
        }

        public static float ToRadians(double degrees)
        {
            return (float)(degrees * Pi / 180);
        }
    }
}