using System;

namespace ComponentFramework.Tools
{
    public static class ArrayHelper
    {
        public static bool In<T>(T value, params T[] values)
        {
            foreach (T compared in values)
                if (value.Equals(compared)) return true;

            return false;
        }

        public static T Coalesce<T>(params T[] values)
        {
            if (values.Length == 1)
                return values[0];

            T defaultValue = default(T);
            foreach (T value in values)
                if (!value.Equals(defaultValue))
                    return value;

            return defaultValue;
        }

        public static T Next<T>(this T[] values, T current)
        {
            var currentIndex = Array.IndexOf(values, current);
            return values[Math.Min(currentIndex + 1, values.Length - 1)];
        }

        public static T Previous<T>(this T[] values, T current)
        {
            var currentIndex = Array.IndexOf(values, current);
            return values[Math.Max(currentIndex - 1, 0)];
        }
    }
}
