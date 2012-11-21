using System;
using ComponentFramework.Tools;
using SlimDX;

namespace ComponentFramework.Structures
{
    public struct ThumbstickState
    {
        const double PressThreshold = 0.5;

        public readonly Vector2 Position;
        public readonly Vector2 Movement;
        public readonly TimedButtonState Clicked;

        public readonly TimedButtonState Up;
        public readonly TimedButtonState Down;
        public readonly TimedButtonState Left;
        public readonly TimedButtonState Right;

        ThumbstickState(Vector2 position, Vector2 movement, TimedButtonState clicked, TimedButtonState up, TimedButtonState down, TimedButtonState left, TimedButtonState right)
        {
            Position = position;
            Movement = movement;
            Clicked = clicked;
            Up = up;
            Down = down;
            Left = left;
            Right = right;
        }

        internal ThumbstickState NextState(Vector2 position, bool clicked, TimeSpan elapsed)
        {
            return new ThumbstickState(position, position - Position,
                                       Clicked.NextState(clicked, elapsed),
                                       Up.NextState(MathHelper.Saturate(position.Y) > PressThreshold, elapsed),
                                       Down.NextState(MathHelper.Saturate(-position.Y) > PressThreshold, elapsed),
                                       Left.NextState(MathHelper.Saturate(-position.X) > PressThreshold, elapsed),
                                       Right.NextState(MathHelper.Saturate(position.X) > PressThreshold, elapsed));
        }

        public override string ToString()
        {
            return StringHelper.ReflectToString(this);
        }
    }
}
