using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Properties;

namespace Cubica.Components.Objects
{
    partial class Water : ObjectBase
    {
        private TVMesh mesh;
        private TV_PLANE plane;

        public TVRenderSurface ReflectRS;
        public TVRenderSurface RefractRS;

        public Water(ICore core) : base(core) { }

        public override void Initialize()
        {
            ReflectRS = Scene.CreateRenderSurfaceEx(-1, -1, CONST_TV_RENDERSURFACEFORMAT.TV_TEXTUREFORMAT_DEFAULT, true, true, 1);
            ReflectRS.SetBackgroundColor(Globals.RGBA(0f, 0f, 0.1906f, 1f));

            RefractRS = Scene.CreateRenderSurfaceEx(-1, -1, CONST_TV_RENDERSURFACEFORMAT.TV_TEXTUREFORMAT_DEFAULT, true, true, 1);
            RefractRS.SetBackgroundColor(Globals.RGBA(0f, 0f, 0.1906f, 1f));

            mesh = Core.Scene.CreateMeshBuilder();
            mesh.AddFloor(Helpers.GetDUDVTextureFromResource(Core, Resources.water), -256, -256, 256, 256, -3, 2, 2);
            mesh.SetPosition(Position.x, Position.y, Position.z);
            mesh.SetScale(Scale.x, Scale.y, Scale.z);

            plane = new TV_PLANE(Globals.Vector3(0, 1, 0), 3f);
            GraphicEffect.SetWaterReflection(mesh, ReflectRS, RefractRS, 0, plane);
        }

        public override void Dispose()
        {
            ReflectRS.Destroy();
            RefractRS.Destroy();
            mesh.Destroy();
        }

        public override void Draw()
        {

        }
    }
}
