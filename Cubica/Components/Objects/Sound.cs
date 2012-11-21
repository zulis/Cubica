using System;
using ComponentFramework.Core;
using IrrKlang;
using Cubica.Managers;
using System.Globalization;

namespace Cubica.Components.Objects
{
    partial class Sound : ObjectBase
    {
        public bool Is3D { get; set; }
        public bool Loop { get; set; }
        public bool Stopped { get; set; }
        public float Volume { get; set; }
        public ISoundSource iSourceSource { get; set; }
        public ISound iSound { get; set; }
        public bool IsFromScript { get; set; }

        public Sound(ICore core) : base(core) { }

        public override void Initialize()
        {
            SoundManager.Load(this);
            SoundManager.SetVolume(this);
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
            iSourceSource.Dispose();
            iSound.Dispose();
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (ScriptEnabled)
            {
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_UPDATE));
            }
        }

        [ServiceDependency]
        public new ISoundManagerService SoundManager { private get; set; }
    }
}
