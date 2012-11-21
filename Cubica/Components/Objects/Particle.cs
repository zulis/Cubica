using System;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Managers;
using System.Globalization;

namespace Cubica.Components.Objects
{
    partial class Particle : ObjectBase
    {
        TVParticleSystem particle;

        public Particle(ICore core) : base(core) { }

        public override void Initialize()
        {
            particle = Scene.CreateParticleSystem();
            particle.Load(FileName);
            particle.SetGlobalPosition(Position.x, Position.y, Position.z);
            particle.SetGlobalScale(Scale.x, Scale.y, Scale.z);
            particle.Enable(Visible);

            // Register object in Lua.
            ScriptManager.SetGlobal(Name, this);
        }

        public override void PostInitialize()
        {
            if (ScriptEnabled)
            {
                ScriptManager.RegisterCustomFunctions(this);
                ScriptManager.DoString(Script);
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_INIT));
            }
        }

        public override void Dispose()
        {
            particle.Destroy();
            particle = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (ScriptEnabled)
            {
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_UPDATE));
            }
        }

        public override void Draw()
        {
            if (ScriptEnabled)
            {
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_DRAW));
            }
        }

        [ServiceDependency]
        public new ISceneManagerService SceneManager { private get; set; }
    }
}
