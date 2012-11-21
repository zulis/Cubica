using System;
using System.IO;
using System.Windows.Forms;
using ComponentFramework.Components;
using ComponentFramework.Core;
using Cubica.Components.Camera;
using Cubica.Components.Objects;
using System.Globalization;

namespace Cubica.Managers
{
    [AutoLoad]
    partial class GameManager : Component, IGameManagerService
    {
        public GameManager(ICore core) : base(core) { }

        public override void Initialize()
        {
            // Register object in Lua.
            ScriptManager.SetGlobal("Game", this);

            var mainLuaFile = Path.Combine(Core.ContentRoot, @"Scripts\main.lua");
            if (File.Exists(mainLuaFile))
            {
                Camera.SetPosition(Core.Settings.StartupCameraPosition);
                Camera.SetLookAt(Core.Settings.StartupCameraLookAt);

                if (File.Exists(Path.Combine(Application.StartupPath, Core.Settings.StartupScene)))
                    ScriptManager.SetGlobal("startupScene", Core.Settings.StartupScene);
                else
                    DebuggingBag.Put(string.Format(CultureInfo.InvariantCulture, "Startup scene {0} doesn't exit.", Core.Settings.StartupScene), null);
                
                ScriptManager.DoFile(mainLuaFile);
                // Call post initialize for all game objects.
                SceneManager.GetGameObjects().ForEach(i => (i as ObjectBase).PostInitialize());
            }
            else
                return;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            ScriptManager.CallFunction(Constants.FUNCTION_LOOP);
        }

        public void Reload()
        {
            this.Initialize();
        }

        [ServiceDependency]
        public new ICameraService Camera { private get; set; }
        [ServiceDependency]
        public new IScriptManagerService ScriptManager { private get; set; }
        [ServiceDependency]
        public new ISceneManagerService SceneManager { private get; set; }
        [ServiceDependency]
        public new IDebuggingBagService DebuggingBag { private get; set; }
    }

    public interface IGameManagerService : IService
    {
        void Reload();
    }
}
