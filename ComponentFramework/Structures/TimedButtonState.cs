using System;
using ComponentFramework.Tools;

namespace ComponentFramework.Structures
{
    public struct TimedButtonState
    {
        public readonly TVButtonState State;
        public readonly TimeSpan TimePressed;

        TimedButtonState(TVButtonState state, TimeSpan timePressed)
        {
            State = state;
            TimePressed = timePressed;
        }

        internal TimedButtonState NextState(bool down, TimeSpan elapsed)
        {
            return new TimedButtonState(State.NextState(down), down ? TimePressed + elapsed : TimeSpan.Zero);
        }

        public override string ToString()
        {
            return StringHelper.ReflectToString(this);
        }
    }
}