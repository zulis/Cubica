using System;
using System.Windows.Forms;
using ComponentFramework.Core;
using SlimDX;
using SlimDX.DirectInput;

namespace ComponentFramework.Components
{
    public class JoyStick : Component, IJoyStickService
    {
        private Joystick joystick;
        private JoystickState state;
        private int buttonDown;
        private bool joystickIsReady;
        private Timer timer;
        private bool getInputData;

        public JoyStick(ICore core) : base(core) { }

        public override void Initialize()
        {
            buttonDown = -1;
            joystickIsReady = false;

            // Make sure that DirectInput has been initialized
            DirectInput dinput = new DirectInput();
            state = new JoystickState();

            // search for devices
            foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // create the device
                try
                {
                    joystick = new Joystick(dinput, device.InstanceGuid);
                    joystick.SetCooperativeLevel(Core.Settings.RenderForm.Handle,
                        CooperativeLevel.Exclusive | CooperativeLevel.Foreground);
                    break;
                }
                catch (DirectInputException)
                {
                }
            }

            if (joystick != null)
            {
                foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
                {
                    if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                        joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                }
            }
            else
            {
                return;
            }

            // acquire the device
            joystick.Acquire();

            timer = new Timer();
            timer.Interval = 1000 / 10;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        public override void Dispose()
        {
            if (timer != null)
                timer.Stop();

            if (joystick != null)
            {
                joystick.Unacquire();
                joystick.Dispose();
            }

            joystick = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            buttonDown = -1;

            if (getInputData)
                getInputData = false;
            else
                return;

            if (joystick != null)
            {
                if (joystick.Acquire().IsFailure ||
                    joystick.Poll().IsFailure)
                    return;

                state = joystick.GetCurrentState();

                if (Result.Last.IsFailure)
                    return;

                joystickIsReady = true;

                bool[] buttons = state.GetButtons();

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i])
                    {
                        buttonDown = i;
                        break;
                    }
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            getInputData = true;
        }

        public bool JoyButtonDown(int button)
        {
            if (!joystickIsReady)
                return false;
            else
                return button.Equals(buttonDown);
        }

        public bool JoyButtonDownAny()
        {
            if (!joystickIsReady)
                return false;
            else
                return !buttonDown.Equals(-1);
        }

        public bool JoyUpKeyDown()
        {
            if (!joystickIsReady)
                return false;
            else
            {
                if (state.Y.Equals(-1000))
                {
                    state = new JoystickState();
                    return true;
                }
                else
                    return false;
            }
        }

        public bool JoyDownKeyDown()
        {
            if (!joystickIsReady)
                return false;
            else
            {
                if (state.Y.Equals(1000))
                {
                    state = new JoystickState();
                    return true;
                }
                else
                    return false;
            }
        }

        public bool JoyLeftKeyDown()
        {
            if (!joystickIsReady)
                return false;
            else
            {
                if (state.X.Equals(-1000))
                {
                    state = new JoystickState();
                    return true;
                }
                else
                    return false;
            }
        }

        public bool JoyRightKeyDown()
        {
            if (!joystickIsReady)
                return false;
            else
            {
                if (state.X.Equals(1000))
                {
                    state = new JoystickState();
                    return true;
                }
                else
                    return false;
            }
        }
    }

    public interface IJoyStickService : IService
    {
        bool JoyButtonDown(int button);
        bool JoyButtonDownAny();
        bool JoyUpKeyDown();
        bool JoyDownKeyDown();
        bool JoyLeftKeyDown();
        bool JoyRightKeyDown();
    }
}
