using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AwesomiumDotNet;
using ComponentFramework.Components;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Components.Objects;
using Cubica.Properties;
using System.IO;
using Cubica.Components.Shaders;

namespace Cubica.Managers
{
    public enum UIType
    {
        Main,
        SelectLevel,
        Statistics,
        Options,
        About,
        Quit,
        Win,
        Dead,
        Pause,
        Finished,
        Credits,
        Tutorial,
        InGame
    };

    public enum UIFlags
    {
        None,
        TopCenter,
        LeftCenter,
        RightCenter,
        BottomCenter,
        Center,
        BottomFullWidth,
        BottomRight,
        BottomLeft
    };

    public class UIView : Component
    {
        private WebCore webCore;
        protected WebView View;
        private int width;
        private int height;
        private UIFlags flags;
        private bool isLoading;
        private bool pageLoaded;
        private int webTextureID;
        private TVScreen2DImmediate hud;
        private int screenWidth { get { return Core.Engine.GetViewport().GetWidth(); } }
        private int screenHeight { get { return Core.Engine.GetViewport().GetHeight(); } }
        private float hudPosX;
        private float hudPosY;
        private bool isTransparent;
        private UIType menuType;
        private Sound buttonClickSound;
        private Sound buttonFocusSound;

        public IMouseService Mouse { get; private set; }
        public IKeyboardService Keyboard { get; private set; }
        public IJoyStickService JoyStick { get; private set; }
        public IGamepadsService Gamepad { get; private set; }
        public UIType MenuType { get { return menuType; } }
        public IUIManagerService UIManager { get { return Core.GetService<IUIManagerService>(); } }

        public UIView(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core)
        {
            webCore = core.GetService<IUIManagerService>().GetWebCore();
            this.menuType = menuType;
            this.width = width;
            this.height = height;
            this.flags = flags;
            this.isTransparent = transparent;
            isLoading = false;
            pageLoaded = false;
            webTextureID = TextureFactory.CreateTexture(width, height, isTransparent);
            hudPosX = 0;
            hudPosY = 0;
            hud = new TVScreen2DImmediate();
            Keyboard = core.GetService<IKeyboardService>();
            Mouse = core.GetService<IMouseService>();
            JoyStick = core.GetService<IJoyStickService>();
            Gamepad = core.GetService<IGamepadsService>();

            CanculateHudPosition(flags);

            View = webCore.CreateWebView(width, height, isTransparent, true);
            View.OnFinishLoading += OnFinishLoading;
            View.OnCallback += OnCallback;
            View.Focus();

            buttonClickSound = Core.GetService<ISoundManagerService>().Load2DSound(Path.Combine(Application.StartupPath, @"Data\Sounds\menu\button_click.mp3"));
            buttonFocusSound = Core.GetService<ISoundManagerService>().Load2DSound(Path.Combine(Application.StartupPath, @"Data\Sounds\menu\button_focus.mp3"));
            Core.GetService<ISoundManagerService>().SetVolume(buttonClickSound, 0.5f);
            Core.GetService<ISoundManagerService>().SetVolume(buttonFocusSound, 0.5f);
        }

        public void PlayButtonClickSound()
        {
            Core.GetService<ISoundManagerService>().Play2DSound(buttonClickSound);
        }

        public void PlayButtonFocusSound()
        {
            Core.GetService<ISoundManagerService>().Play2DSound(buttonFocusSound);
        }

        public override void Dispose()
        {
            try
            {
                if (isLoading)
                {
                    ExecuteJavascript("window.stop();");
                    System.Threading.Thread.Sleep(100);
                }

                View.Dispose();
                View = null;
                TextureFactory.DeleteTexture(webTextureID);
            }
            catch (Exception) { }
        }

        public virtual void OnCallback(string name, JSValue[] args)
        {
            PlayButtonClickSound();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            View.InjectKeyboardChar(e.KeyChar);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            View.InjectKeyboardUp(e.KeyCode);
        }

        public void LoadURL(string url)
        {
            isLoading = true;
            pageLoaded = false;
            View.LoadURL(url);
        }

        public void LoadHTML(string html)
        {
            isLoading = true;
            pageLoaded = false;
            View.LoadHTML(html);
        }

        public void LoadFile(string fileName)
        {
            isLoading = true;
            pageLoaded = false;
            View.LoadFile(fileName);
        }

        public void ExecuteJavascript(string javaScript)
        {
            View.ExecuteJavascript(javaScript);
            pageLoaded = false;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            CanculateHudPosition(flags);

            if (UIManager.IsCursorVisible())
            {
                View.InjectMouseMove((int)(Mouse.Position.X - hudPosX), (int)(Mouse.Position.Y - hudPosY));
                View.InjectMouseWheel(Mouse.WheelTurns);

                switch (Mouse.LeftButton.State)
                {
                    case ComponentFramework.Structures.TVButtonState.Up:
                        View.InjectMouseUp(MouseButton.Left);
                        break;
                    case ComponentFramework.Structures.TVButtonState.Down:
                        View.InjectMouseDown(MouseButton.Left);
                        break;
                }
            }
        }

        public override void PostDraw()
        {
            if (!pageLoaded)
                return;

            if ((View != null) && View.IsDirty)
            {
                isLoading = true;
                int depth = 4;
                int rowSpan = width * depth;
                int length = rowSpan * height;
                byte[] buffer = new byte[length];
                View.Render(buffer, rowSpan, depth);
                int[] data = new int[buffer.Length];

                // 1 way
                //IntPtr pointer = Marshal.AllocHGlobal(buffer.Length);
                //Marshal.Copy(buffer, 0, pointer, buffer.Length);
                //Marshal.Copy(pointer, data, 0, width * height);
                //Marshal.FreeHGlobal(pointer);

                // 2 way
                GCHandle pinnedArray = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                Marshal.Copy(pointer, data, 0, width * height);
                pinnedArray.Free();

                TextureFactory.LockTexture(webTextureID, false);
                TextureFactory.SetPixelArray(webTextureID, 0, 0, width, height, data);
                TextureFactory.UnlockTexture(webTextureID, true);
            }

            Screen2DImmediate.Action_Begin2D();
            hud.Draw_Sprite(webTextureID, hudPosX, hudPosY);
            Screen2DImmediate.Action_End2D();
        }

        private void OnFinishLoading()
        {
            pageLoaded = true;
            isLoading = false;
            // To avoid white background in transparent mode we need pause a little bit
            //if (isTransparent)
            //    System.Threading.Thread.Sleep(50);
            //if (isTransparent)
            //    View.ExecuteJavaScript("document.body.style.background = 'transparent';");
            PageLoaded();
        }

        private void CanculateHudPosition(UIFlags flags)
        {
            switch (flags)
            {
                case UIFlags.TopCenter:
                    hudPosX = (screenWidth - width) / 2;
                    hudPosY = 0;
                    break;
                case UIFlags.LeftCenter:
                    hudPosX = 0;
                    hudPosY = (screenHeight - height) / 2;
                    break;
                case UIFlags.RightCenter:
                    hudPosX = screenWidth - width;
                    hudPosY = (screenHeight - height) / 2;
                    break;
                case UIFlags.BottomCenter:
                    hudPosX = (screenWidth - width) / 2;
                    hudPosY = screenHeight - height;
                    break;
                case UIFlags.Center:
                    hudPosX = (screenWidth - width) / 2;
                    hudPosY = (screenHeight - height) / 2;
                    break;
                case UIFlags.BottomRight:
                    hudPosX = screenWidth - width;
                    hudPosY = screenHeight - height;
                    break;
                case UIFlags.BottomLeft:
                    hudPosX = 0;
                    hudPosY = screenHeight - height;
                    break;
                default:
                    hudPosX = 0;
                    hudPosY = 0;
                    break;
            }
        }

        public virtual void Reload()
        {
            ExecuteJavascript("location.reload(true)");
        }

        public virtual void PageLoaded()
        {
        }
    }
}
