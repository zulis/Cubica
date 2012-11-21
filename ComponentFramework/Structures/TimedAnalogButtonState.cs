using System;
using ComponentFramework.Tools;
using SlimDX.XInput;

namespace ComponentFramework.Structures
{
    public struct TimedAnalogButtonState
    {
        const double TriggerThreshold = (double) Gamepad.GamepadTriggerThreshold / byte.MaxValue;

        public readonly float Value;
        public readonly TVButtonState State;
        public readonly TimeSpan TimePressed;

        TimedAnalogButtonState(float value, TVButtonState state, TimeSpan timePressed)
        {
            Value = value;
            State = state;
            TimePressed = timePressed;
        }

        internal TimedAnalogButtonState NextState(float value, TimeSpan elapsed)
        {
            var down = value > TriggerThreshold;
            return new TimedAnalogButtonState(value, State.NextState(down), down ? TimePressed + elapsed : TimeSpan.Zero);
        }

        public override string ToString()
        {
            return StringHelper.ReflectToString(this);
        }
    }
}
