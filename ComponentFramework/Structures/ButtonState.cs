namespace ComponentFramework.Structures
{
    public enum TVButtonState
    {
        Up,
        Pressed,
        Released,
        Down
    }

    public static class ButtonStateExtensions
    {
        public static TVButtonState NextState(this TVButtonState state, bool pressed)
        {
            switch (state)
            {
                case TVButtonState.Up:
                    return pressed ? TVButtonState.Pressed : TVButtonState.Up;
                case TVButtonState.Pressed:
                    return pressed ? TVButtonState.Down : TVButtonState.Released;
                case TVButtonState.Released:
                    return pressed ? TVButtonState.Pressed : TVButtonState.Up;
                default:
                case TVButtonState.Down:
                    return pressed ? TVButtonState.Down : TVButtonState.Released;
            }
        }
    }
}
