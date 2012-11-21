using System;

namespace ComponentFramework.Tools
{
    public static class Easing
    {
        // Adapted from source : http://www.robertpenner.com/easing/

        public static float Ease(double linearStep, float acceleration, EasingType type)
        {
            float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                              acceleration < 0 ? EaseOut(linearStep, type) :
                              (float)linearStep;

            return MathHelper.Lerp((float)linearStep, easedStep, Math.Abs(acceleration));
        }

        public static float EaseIn(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step:       return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear:     return (float)linearStep;
                case EasingType.Sine:       return Sine.EaseIn(linearStep);
                case EasingType.Quadratic:  return Power.EaseIn(linearStep, 2);
                case EasingType.Circular:   return Circular.EaseIn(linearStep);
            }
            throw new NotImplementedException();
        }

        public static float EaseOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step:       return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear:     return (float)linearStep;
                case EasingType.Sine:       return Sine.EaseOut(linearStep);
                case EasingType.Quadratic:  return Power.EaseOut(linearStep, 2);
                case EasingType.Circular:   return Circular.EaseOut(linearStep);
            }
            throw new NotImplementedException();
        }

        public static float EaseInOut(double linearStep, EasingType easeInType, float acceleration, EasingType easeOutType, float deceleration)
        {
            return linearStep < 0.5
                       ? MathHelper.Lerp((float)linearStep, EaseInOut(linearStep, easeInType), acceleration)
                       : MathHelper.Lerp((float)linearStep, EaseInOut(linearStep, easeOutType), deceleration);
        }
        public static float EaseInOut(double linearStep, EasingType easeInType, EasingType easeOutType)
        {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
        }
        public static float EaseInOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step:       return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear:     return (float)linearStep;
                case EasingType.Sine:       return Sine.EaseInOut(linearStep);
                case EasingType.Quadratic:  return Power.EaseInOut(linearStep, 2);
                case EasingType.Circular:   return Circular.EaseInOut(linearStep);
            }
            throw new NotImplementedException();
        }

        static class Sine
        {
            public static float EaseIn(double s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
            }
            public static float EaseOut(double s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi);
            }
            public static float EaseInOut(double s)
            {
                return (float)(Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
            }
        }

        static class Power
        {
            public static float EaseIn(double s, int power)
            {
                return (float)Math.Pow(s, power);
            }
            public static float EaseOut(double s, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(s - 1, power) + sign));
            }
            public static float EaseInOut(double s, int power)
            {
                s *= 2;
                if (s < 1) return EaseIn(s, power) / 2;
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }

        static class Circular
        {
            public static float EaseIn(double s)
            {
                return (float)-(Math.Sqrt(1 - s * s) - 1);
            }
            public static float EaseOut(double s)
            {
                //c * Math.sqrt(1 - (t=t/d-1)*t) + b;
                return (float)Math.Sqrt(1 - Math.Pow(s - 1, 2));
            }
            public static float EaseInOut(double s)
            {
                s *= 2;
                if (s < 1) return EaseIn(s) / 2;
                return (float) (Math.Sqrt(1 - Math.Pow(s - 2, 2)) + 1) / 2;
            }
        }
    }

    public enum EasingType
    {
        Step,
        Linear,
        Sine,
        Quadratic,
        Circular
    }
}
