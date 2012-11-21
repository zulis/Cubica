using System;
using ComponentFramework.Tools;
using SlimDX;

namespace ComponentFramework.Structures
{
    public struct DirectionalState
    {
        public readonly Vector2 Direction;
        public readonly Vector2 Movement;

        public readonly TimedButtonState Up;
        public readonly TimedButtonState Down;
        public readonly TimedButtonState Left;
        public readonly TimedButtonState Right;

        DirectionalState(Vector2 direction, Vector2 movement, TimedButtonState up, TimedButtonState down, TimedButtonState left, TimedButtonState right) 
        {
            Direction = direction;
            Movement = movement;
            Up = up;
            Down = down;
            Left = left;
            Right = right;
        }

        internal DirectionalState NextState(bool up, bool down, bool left, bool right, TimeSpan elapsed)
        {
            var direction = new Vector2(left ? -1 : right ? 1 : 0, up ? 1 : down ? -1 : 0);
            return new DirectionalState(direction, direction - Direction,
                                       Up.NextState(up, elapsed),
                                       Down.NextState(down, elapsed),
                                       Left.NextState(left, elapsed),
                                       Right.NextState(right, elapsed));
        }

        public override string ToString()
        {
            return StringHelper.ReflectToString(this);
        }
    }
}