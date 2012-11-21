using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ComponentFramework.Components;
using ComponentFramework.Core;
using Cubica.Managers;
using MTV3D65;

namespace Cubica
{
    public class Program : Core
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            
            var settings = new EngineSettings();
            settings.VSync = true;
            settings.UseGlowEffect = true;
            //settings.UpdateFrequency = 1f / 60f;
            settings.MultiThreading = true;
            settings.MultisampleType = Helpers.GetMultisample(Helpers.GameSettings.Antialiasing);
            settings.Fullscreen = Helpers.GameSettings.FullScreen;
            settings.ScreenMode = Helpers.GameSettings.ScreenMode;

            if (args.Length > 0)
            {
                var startupScene = args[0];
                var startupCameraPosition = new TV_3DVECTOR(float.Parse(args[1], CultureInfo.InvariantCulture),
                    float.Parse(args[2], CultureInfo.InvariantCulture),
                    float.Parse(args[3], CultureInfo.InvariantCulture));
                var startupCameraLookAt = new TV_3DVECTOR(float.Parse(args[4], CultureInfo.InvariantCulture),
                    float.Parse(args[5], CultureInfo.InvariantCulture),
                    float.Parse(args[6], CultureInfo.InvariantCulture));
                settings.StartupScene = startupScene;
                settings.StartupCameraPosition = startupCameraPosition;
                settings.StartupCameraLookAt = startupCameraLookAt;
                settings.PreviewMode = true;
                settings.Fullscreen = false;
            }
            else
            {
                settings.StartupScene = "data/scenes/main.xml";
            }

            try
            {
                new Program().Run(settings);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                message += ex.StackTrace;
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Initialize()
        {
            ContentRoot = Path.Combine(Application.StartupPath, "Data");
            Settings.RenderForm.Text = "Cubica";
            var deviceInfo = new TVDeviceInfo();
            Engine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_DEGREE);
            Scene.SetViewFrustum(65f, 2000f);

            CreateAndLoadComponent<DebuggingBag>();
            //CreateAndLoadComponent<ScriptManager>();
            CreateAndLoadComponent<Keyboard>();
            CreateAndLoadComponent<Mouse>();
            CreateAndLoadComponent<JoyStick>();
            CreateAndLoadComponent<Gamepads>();
            //CreateAndLoadComponent<PersistentThreadPool>();
            //CreateAndLoadComponent<SoundPlayer>();
            //CreateAndLoadComponent<GameManager>();

            base.Initialize();
        }
    }
}
