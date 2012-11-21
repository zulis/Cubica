using System;
using System.Windows.Forms;
using ComponentFramework.Core;
using ComponentFramework.Structures;
using SlimDX;
using SlimDX.DirectInput;

namespace ComponentFramework.Components
{
    public class Mouse : Component, IMouseService
    {
        DirectInput dinput;
        Vector2 position;
        CooperativeLevel cooperativeLevel;

        public Mouse(ICore core) : base(core) 
        {
            Cursor.Current = Cursors.Default;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (Core.Settings.Fullscreen)
            {
                InputEngine.SetMousePosition(Cursor.Position.X, Cursor.Position.Y);
            }

            int x = 0, y = 0, dX = 0, dY = 0, roll = 0;
            bool button1 = false, button2 = false, button3 = false, button4 = false;

            InputEngine.GetMouseState(ref dX, ref dY, ref button1, ref button2, ref button3, ref button4, ref roll);
            InputEngine.GetMousePosition(ref x, ref y);

            position = new Vector2(x, y);
            Movement = new Vector2(dX, dY);

            LeftButton = LeftButton.NextState(button1, elapsedTime);
            RightButton = RightButton.NextState(button2, elapsedTime);
            MiddleButton = MiddleButton.NextState(button3, elapsedTime);

            WheelTurns = roll;
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                InputEngine.SetMousePosition((int)position.X, (int)position.Y);
            }
        }

        public Vector2 Movement { get; private set; }

        public TimedButtonState LeftButton { get; private set; }
        public TimedButtonState MiddleButton { get; private set; }
        public TimedButtonState RightButton { get; private set; }

        public int WheelTurns { get; private set; }
    }

    public interface IMouseService : IService
    {
        Vector2 Position { get; set; }
        Vector2 Movement { get; }

        TimedButtonState LeftButton { get; }
        TimedButtonState MiddleButton { get; }
        TimedButtonState RightButton { get; }

        int WheelTurns { get; }
    }
}
