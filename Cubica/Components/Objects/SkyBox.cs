using System.IO;
using System.Windows.Forms;
using ComponentFramework.Core;

namespace Cubica.Components.Objects
{
    partial class SkyBox : ObjectBase
    {
        public string FrontTexture { get; set; }
        public string BackTexture { get; set; }
        public string LeftTexture { get; set; }
        public string RightTexture { get; set; }
        public string TopTexture { get; set; }
        public string BottomTexture { get; set; }

        public SkyBox(ICore core) : base(core) { }

        public override void Initialize()
        {
            var frontId = TextureFactory.LoadTexture(Path.Combine(Application.StartupPath, FrontTexture));
            var backId = TextureFactory.LoadTexture(Path.Combine(Application.StartupPath, BackTexture));
            var leftId = TextureFactory.LoadTexture(Path.Combine(Application.StartupPath, LeftTexture));
            var rightId = TextureFactory.LoadTexture(Path.Combine(Application.StartupPath, RightTexture));
            var topId = TextureFactory.LoadTexture(Path.Combine(Application.StartupPath, TopTexture));
            var bottomId = TextureFactory.LoadTexture(Path.Combine(Application.StartupPath, BottomTexture));

            Atmosphere.SkyBox_Enable(true);
            Atmosphere.SkyBox_SetTexture(frontId, backId, leftId, rightId, topId, bottomId);
        }

        public override void Draw()
        {
            Atmosphere.SkyBox_Render();
        }
    }
}
