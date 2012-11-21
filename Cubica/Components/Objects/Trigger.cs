using System;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Managers;
using System.Globalization;

namespace Cubica.Components.Objects
{
    partial class Trigger : ObjectBase
    {
        public bool Show { get; set; }
        public int Color { get; set; }

        TV_3DVECTOR boundingBoxMin;
        TV_3DVECTOR boundingBoxMax;
        public TV_3DVECTOR BoundingBoxMin { get { return boundingBoxMin; } }
        public TV_3DVECTOR BoundingBoxMax { get { return boundingBoxMax; } }

        public Trigger(ICore core) : base(core) { }

        public override void Initialize()
        {
#if DEBUG
            Show = true;
#endif
            TVMesh mesh = Scene.CreateMeshBuilder(Name);
            mesh.CreateBox(1, 1, 1);
            mesh.SetPosition(Position.x, Position.y, Position.z);
            mesh.SetRotation(Rotation.x, Rotation.y, Rotation.z);
            mesh.SetScale(Scale.x, Scale.y, Scale.z);
            mesh.GetBoundingBox(ref boundingBoxMin, ref boundingBoxMax);
            mesh.Destroy();
            mesh = null;

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

        public override void Update(TimeSpan elapsedTime)
        {
            if (ScriptEnabled)
            {
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_UPDATE));
            }
        }

        public override void Draw()
        {
            if (Show)
            {
                Screen2DImmediate.Draw_Box3D(boundingBoxMin, boundingBoxMax, Color);
            }
        }

        [ServiceDependency]
        public new IScriptManagerService ScriptManager { private get; set; }
    }
}
