using System.IO;
using System.Windows.Forms;
using MTV3D65;

namespace ComponentFramework.Core
{
    /// <summary>
    /// The engine initialization settings.
    /// The whole structure does not have to be filled, but certain illegal combinations will
    /// cause exceptions to be raised.
    /// </summary>
    public class EngineSettings
    {
        /// <summary>
        /// Public parameterless constructor.
        /// Use properties for construction settings.
        /// </summary>
        public EngineSettings()
        {
            DebugFile = new FileInfo(Application.StartupPath + @"\Debug.txt");
        }

        /// <summary>
        /// Internal copy constructor.
        /// </summary>
        /// <param name="source"></param>
        internal EngineSettings(EngineSettings source)
        {
            MultiThreading = source.MultiThreading;
            DebugFile = source.DebugFile;
            RenderForm = source.RenderForm;
            ScreenMode = source.ScreenMode;
            Fullscreen = source.Fullscreen;
            VSync = source.VSync;
            MultisampleType = source.MultisampleType;
            StartupScene = string.Empty;
            PreviewMode = false;
            UpdateFrequency = source.UpdateFrequency;
            UseGlowEffect = source.UseGlowEffect;
        }

        /// <summary>
        /// Whether multi-threading mode of the TV3D engine is used.
        /// </summary>
        public bool MultiThreading;

        /// <summary>
        /// The debug file location, Debug.txt in the application startup path as default.
        /// </summary>
        public FileInfo DebugFile;

        /// <summary>
        /// The render form.
        /// </summary>
        public Form RenderForm;

        /// <summary>
        /// The screen mode format.
        /// </summary>
        public TV_MODEFORMAT ScreenMode;

        /// <summary>
        /// Whether full screen mode is used.
        /// </summary>
        public bool Fullscreen;

        /// <summary>
        /// Whether vertical synchronization is used.
        /// </summary>
        public bool VSync;

        /// <summary>
        /// The multi sampling (MSAA) type used, <see cref="CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_NONE"/> if none is wanted.
        /// </summary>
        public CONST_TV_MULTISAMPLE_TYPE MultisampleType;

        /// <summary>
        /// Start game with scene.
        /// </summary>
        public string StartupScene;

        /// <summary>
        /// Start game camera position.
        /// </summary>
        public TV_3DVECTOR StartupCameraPosition;

        /// <summary>
        /// Start game camera look at.
        /// </summary>
        public TV_3DVECTOR StartupCameraLookAt;

        /// <summary>
        /// Indicates if we came from editor or not.
        /// </summary>
        public bool PreviewMode;

        /// <summary>
        /// Component update frequency (fixed hertz).
        /// </summary>
        public float UpdateFrequency;

        /// <summary>
        /// Whether TV3D engines glow effect is used.
        /// </summary>
        public bool UseGlowEffect;
    }
}