using System;
using System.Drawing;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Managers;
using System.Globalization;

namespace Cubica.Components.Objects
{
    /// <summary>
    /// Directional light class.
    /// </summary>
    partial class DirectionalLight : ObjectBase
    {
        public int LightId;
        public TV_3DVECTOR Direction { get; set; }
        public Color Color { get; set; }

        public DirectionalLight(ICore core) : base(core) { }

        public override void Initialize()
        {
            LightId = LightEngine.CreateDirectionalLight(Direction, Color.R / 255f, this.Color.G / 255f, this.Color.B / 255f);
            LightEngine.SetLightProperties(LightId, true, false, false);
            
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
            LightEngine.DeleteLight(LightId);
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
    }
}
