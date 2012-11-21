using ComponentFramework.Components;
using ComponentFramework.Structures;
using MTV3D65;
using Cubica.Managers;

namespace Cubica.Components.Camera
{
    partial class Camera
    {
        [RegisterFunction]
        public bool KeyDown(string key)
        {
            return Keyboard.GetKeyState(Keyboard.GetTVKey(key)) == TVButtonState.Down;
        }

        [RegisterFunction]
        public bool KeyPressed(string key)
        {
            return Keyboard.GetKeyState(Keyboard.GetTVKey(key)) == TVButtonState.Pressed;
        }

        [RegisterFunction]
        public bool KeyReleased(string key)
        {
            return Keyboard.GetKeyState(Keyboard.GetTVKey(key)) == TVButtonState.Released;
        }

        [RegisterFunction]
        public bool KeyUp(string key)
        {
            return Keyboard.GetKeyState(Keyboard.GetTVKey(key)) == TVButtonState.Up;
        }

        [RegisterFunction]
        public bool JoyButtonDown(int button)
        {
            return JoyStick.JoyButtonDown(button);
        }

        [RegisterFunction]
        public bool JoyButtonDownAny()
        {
            return JoyStick.JoyButtonDownAny();
        }

        [RegisterFunction]
        public bool JoyUpKeyDown()
        {
            return JoyStick.JoyUpKeyDown();
        }

        [RegisterFunction]
        public bool JoyDownKeyDown()
        {
            return JoyStick.JoyDownKeyDown();
        }

        [RegisterFunction]
        public bool JoyLeftKeyDown()
        {
            return JoyStick.JoyLeftKeyDown();
        }

        [RegisterFunction]
        public bool JoyRightKeyDown()
        {
            return JoyStick.JoyRightKeyDown();
        }

        [RegisterFunction]
        public bool GamepadUpKeyDown()
        {
            return Gamepad[0].DPad.Down.State == TVButtonState.Down;
        }

        [RegisterFunction]
        public bool GamepadDownKeyDown()
        {
            return Gamepad[0].DPad.Up.State == TVButtonState.Down;
        }

        [RegisterFunction]
        public bool GamepadLeftKeyDown()
        {
            return Gamepad[0].DPad.Left.State == TVButtonState.Down;
        }

        [RegisterFunction]
        public bool GamepadRightKeyDown()
        {
            return Gamepad[0].DPad.Right.State == TVButtonState.Down;
        }

        [RegisterFunction]
        public void GamepadVibrate()
        {
            Gamepad[0].Vibrate(VibrationMotor.LeftLow, 1, new System.TimeSpan(0, 0, 1));
        }

        [RegisterFunction]
        public TV_3DVECTOR GetPosition()
        {
            return camera.GetPosition();
        }

        [RegisterFunction]
        public void SetPosition(TV_3DVECTOR position)
        {
            SetPosition(position.x, position.y, position.z);
        }

        [RegisterFunction]
        public void SetPosition(float x, float y, float z)
        {
            camera.SetPosition(x, y, z);
        }

        [RegisterFunction]
        public void SetLookAt(TV_3DVECTOR lookAt)
        {
            SetLookAt(lookAt.x, lookAt.y, lookAt.z);
        }

        [RegisterFunction]
        public void SetLookAt(float x, float y, float z)
        {
            camera.SetLookAt(x, y, z);
        }

        [RegisterFunction]
        public void SetViewFrustum(float value)
        {
            camera.SetViewFrustum(value, 1000, 0.01f);
        }

        [RegisterFunction]
        public void RotateX(float value)
        {
            camera.RotateX(value);
        }

        [RegisterFunction]
        public void RotateY(float value)
        {
            camera.RotateY(value);
        }

        [RegisterFunction]
        public void RotateZ(float value)
        {
            camera.RotateZ(value);
        }
    }
}
