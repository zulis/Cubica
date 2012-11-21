using ComponentFramework.Components;
using ComponentFramework.Core;
using Cubica.Managers;
using MTV3D65;

namespace Cubica.Components.Camera
{
    [AutoLoad]
    partial class Camera : Component, ICameraService
    {
        TVCamera camera;

        public Camera(ICore core) : base(core) { Order = int.MinValue + 10; }

        public override void Initialize()
        {
            camera = CameraFactory.GetCamera(0);
            camera.SetPosition(0, 15, -25);
            camera.SetLookAt(0, 0, 0);

            // Register object in Lua.
            ScriptManager.SetGlobal("Camera", this);
            ScriptManager.RegisterCustomFunctions(this);
        }

        [ServiceDependency]
        public new IKeyboardService Keyboard { private get; set; }
        [ServiceDependency]
        public new IMouseService Mouse { private get; set; }
        [ServiceDependency]
        public new IJoyStickService JoyStick { private get; set; }
        [ServiceDependency]
        public new IGamepadsService Gamepad { private get; set; }
        [ServiceDependency]
        public new IScriptManagerService ScriptManager { private get; set; }
    }

    public interface ICameraService : IService
    {
        void SetPosition(TV_3DVECTOR position);
        void SetLookAt(TV_3DVECTOR lookAt);
    }
}