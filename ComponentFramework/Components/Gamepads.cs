using System;
using System.Collections.Generic;
using ComponentFramework.Core;
using ComponentFramework.Structures;
using ComponentFramework.Tools;
using SlimDX;
using SlimDX.XInput;

namespace ComponentFramework.Components
{
    public class Gamepads : Component, IGamepadsService
    {
        readonly Dictionary<UserIndex, GamepadState> gamepadStates = new Dictionary<UserIndex, GamepadState>(EnumComparer<UserIndex>.Instance);

        public Gamepads(ICore core) : base(core)
        {
            gamepadStates.Add(UserIndex.One, new GamepadState(UserIndex.One));
            gamepadStates.Add(UserIndex.Two, new GamepadState(UserIndex.Two));
            gamepadStates.Add(UserIndex.Three, new GamepadState(UserIndex.Three));
            gamepadStates.Add(UserIndex.Four, new GamepadState(UserIndex.Four));
        }

        public override void Update(TimeSpan elapsed)
        {
            gamepadStates[UserIndex.One].Update(elapsed);
            gamepadStates[UserIndex.Two].Update(elapsed);
            gamepadStates[UserIndex.Three].Update(elapsed);
            gamepadStates[UserIndex.Four].Update(elapsed);
        }

        public GamepadState this[UserIndex index]
        {
            get { return gamepadStates[index]; }
        }
    }

    public interface IGamepadsService : IService
    {
        GamepadState this[UserIndex index] { get; }
    }

    public class GamepadState 
    {
        VibrationMotorState leftMotor, rightMotor;

        public GamepadState(UserIndex userIndex)
        {
            UserIndex = userIndex;
            Controller = new Controller(userIndex);
        }

        public readonly UserIndex UserIndex;
        public readonly Controller Controller;

        public DirectionalState DPad { get; private set; }
        public ThumbstickState LeftStick { get; private set; }
        public ThumbstickState RightStick { get; private set; }

        public TimedButtonState A { get; private set; }
        public TimedButtonState B { get; private set; }
        public TimedButtonState X { get; private set; }
        public TimedButtonState Y { get; private set; }

        public TimedButtonState RightShoulder { get; private set; }
        public TimedButtonState LeftShoulder { get; private set; }

        public TimedAnalogButtonState RightTrigger { get; private set; }
        public TimedAnalogButtonState LeftTrigger { get; private set; }

        public TVButtonState Start { get; private set; }
        public TVButtonState Back { get; private set; }

        public bool Connected
        {
            get { return Controller.IsConnected; }
        }

        public void Update(TimeSpan elapsed)
        {
            if (!Connected) return;

            State state = Controller.GetState();
            var gamepadState = state.Gamepad;

            // Sadly we can't really use the packet information because everything is temporal
            // Even if everything stayed the same, that's valid data (elapsed time goes up, etc.)

            // Vibration
            if (leftMotor.Active)   leftMotor = UpdateMotor(leftMotor, elapsed);
            if (rightMotor.Active)  rightMotor = UpdateMotor(rightMotor, elapsed);

            if (leftMotor.LastAmount != leftMotor.CurrentAmount || rightMotor.LastAmount != rightMotor.CurrentAmount)
                Controller.SetVibration(new Vibration
                {
                    LeftMotorSpeed = (ushort) (leftMotor.CurrentAmount * ushort.MaxValue),
                    RightMotorSpeed = (ushort) (rightMotor.CurrentAmount * ushort.MaxValue)
                });

            // Shoulders
            LeftShoulder = LeftShoulder.NextState((gamepadState.Buttons & GamepadButtonFlags.LeftShoulder) != 0, elapsed);
            RightShoulder = RightShoulder.NextState((gamepadState.Buttons & GamepadButtonFlags.RightShoulder) != 0, elapsed);

            // Triggers
            LeftTrigger = LeftTrigger.NextState(gamepadState.LeftTrigger / (float)byte.MaxValue, elapsed);
            RightTrigger = RightTrigger.NextState(gamepadState.RightTrigger / (float)byte.MaxValue, elapsed);

            // Buttons
            Start = Start.NextState((gamepadState.Buttons & GamepadButtonFlags.Start) != 0);
            Back = Back.NextState((gamepadState.Buttons & GamepadButtonFlags.Back) != 0);

            A = A.NextState((gamepadState.Buttons & GamepadButtonFlags.A) != 0, elapsed);
            B = B.NextState((gamepadState.Buttons & GamepadButtonFlags.B) != 0, elapsed);
            X = X.NextState((gamepadState.Buttons & GamepadButtonFlags.X) != 0, elapsed);
            Y = Y.NextState((gamepadState.Buttons & GamepadButtonFlags.Y) != 0, elapsed);

            // D-Pad
            DPad = DPad.NextState((gamepadState.Buttons & GamepadButtonFlags.DPadUp) != 0,
                                  (gamepadState.Buttons & GamepadButtonFlags.DPadDown) != 0,
                                  (gamepadState.Buttons & GamepadButtonFlags.DPadLeft) != 0,
                                  (gamepadState.Buttons & GamepadButtonFlags.DPadRight) != 0, elapsed);

            // Thumb sticks
            LeftStick = LeftStick.NextState(
                    Normalize(gamepadState.LeftThumbX, gamepadState.LeftThumbY, Gamepad.GamepadLeftThumbDeadZone),
                    (gamepadState.Buttons & GamepadButtonFlags.LeftThumb) != 0, elapsed);
            RightStick = RightStick.NextState(
                    Normalize(gamepadState.RightThumbX, gamepadState.RightThumbY, Gamepad.GamepadRightThumbDeadZone),
                    (gamepadState.Buttons & GamepadButtonFlags.RightThumb) != 0, elapsed);
        }

        static Vector2 Normalize(short rawX, short rawY, short threshold)
        {
            var value = new Vector2(rawX, rawY);
            var magnitude = value.Length();
            var direction = value / (magnitude == 0 ? 1 : magnitude);

            var normalizedMagnitude = 0.0f;
            if (magnitude - threshold > 0)
                normalizedMagnitude = Math.Min((magnitude - threshold) / (short.MaxValue - threshold), 1);

            return direction * normalizedMagnitude;
        }

        static VibrationMotorState UpdateMotor(VibrationMotorState motorState, TimeSpan elapsedTime)
        {
            if (motorState.ElapsedTime <= motorState.Duration)
            {
                var step = Easing.EaseIn(1 - motorState.ElapsedTime.TotalSeconds / motorState.Duration.TotalSeconds, motorState.EasingType);
                motorState.CurrentAmount = step * motorState.MaximumAmount;
            }
            else
            {
                motorState.CurrentAmount = 0;
                motorState.Active = false;
            }
            motorState.ElapsedTime += elapsedTime;

            return motorState;
        }

        public void Vibrate(VibrationMotor motor, double amount, TimeSpan duration)
        {
            Vibrate(motor, amount, duration, EasingType.Linear);
        }
        public void Vibrate(VibrationMotor motor, double amount, TimeSpan duration, EasingType easingType)
        {
            var motorState = new VibrationMotorState(amount, duration, easingType);
            switch (motor)
            {
                case VibrationMotor.LeftLow:    leftMotor = motorState; break;
                case VibrationMotor.RightHigh:  rightMotor = motorState; break;
            }
        }

        struct VibrationMotorState
        {
            public readonly float MaximumAmount;
            public readonly TimeSpan Duration;
            public readonly EasingType EasingType;

            public bool Active;
            public TimeSpan ElapsedTime;

            public float LastAmount { get; private set; }

            float currentAmount;
            public float CurrentAmount 
            {
                get { return currentAmount; }
                set
                {
                    LastAmount = currentAmount;
                    currentAmount = value;
                }
            }

            public VibrationMotorState(double maximumAmount, TimeSpan duration, EasingType easingType) : this()
            {
                Active = true;
                LastAmount = CurrentAmount = 0;
                ElapsedTime = TimeSpan.Zero;
                MaximumAmount = MathHelper.Saturate(maximumAmount);
                Duration = duration;
                EasingType = easingType;
            }
        }
    }

    public enum VibrationMotor
    {
        None,
        LeftLow, 
        RightHigh
    }
}
