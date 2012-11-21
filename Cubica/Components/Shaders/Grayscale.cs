using System;
using ComponentFramework.Components;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Managers;
using System.IO;
using System.Windows.Forms;

namespace Cubica.Components.Shaders
{
    //[AutoLoad]
    public class Grayscale : Component
    {
        static TVShader testFS;
        static TVRenderSurface testRS;
        int testRSID;

        public Grayscale(ICore core) : base(core) { Order = int.MaxValue; }

        public override void Initialize()
        {
            this.Enabled = false;
            testRS = Core.Scene.CreateRenderSurfaceEx(-1, -1, CONST_TV_RENDERSURFACEFORMAT.TV_TEXTUREFORMAT_A8R8G8B8, true, true, 1);
            testRSID = testRS.GetIndex();
            testFS = Core.Scene.CreateShader();
            bool result = testFS.CreateFromEffectFile(Path.Combine(Application.StartupPath, @"Data\Shaders\Grayscale.fx"));
        }

        public override void Draw()
        {
            testRS.BltFromMainBuffer();
            Core.Screen2DImmediate.Action_Begin2D();
            Core.Screen2DImmediate.Draw_FullscreenQuadWithShader(testFS, 0, 0, 1, 1, testRSID);
            Core.Screen2DImmediate.Action_End2D();
        }
    }
}
