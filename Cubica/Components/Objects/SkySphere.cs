using System.IO;
using System.Windows.Forms;
using ComponentFramework.Core;

namespace Cubica.Components.Objects
{
    partial class SkySphere : ObjectBase
    {
        public int PolyCount;

        public SkySphere(ICore core) : base(core) { }

        public override void Initialize()
        {
            //http://blenderartists.org/forum/showthread.php?24038-Free-high-res-skymaps-%28Massive-07-update!%29
            var textureId = TextureFactory.LoadTexture(Path.Combine(Application.StartupPath, FileName));
            
            Atmosphere.SkySphere_Enable(true);
            Atmosphere.SkySphere_SetTexture(textureId);
            Atmosphere.SkySphere_SetRotation(Rotation.x, Rotation.y, Rotation.z);
            Atmosphere.SkySphere_SetScale(Scale.x, Scale.y, Scale.z);
            Atmosphere.SkySphere_SetPolyCount(PolyCount);
        }

        public override void Draw()
        {
            Atmosphere.SkySphere_Render();
        }
    }
}
