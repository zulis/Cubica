using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using AwesomiumDotNet;
using ComponentFramework.Components;
using ComponentFramework.Core;
using ComponentFramework.Structures;
using Cubica.Components.Objects;
using MTV3D65;

namespace Cubica.Managers
{
    public class UIMain : UIView
    {
        private ICore core;

        public UIMain(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            this.core = core;
            View.AddCallback("PlayClick");
            View.AddCallback("StatisticsClick");
            View.AddCallback("OptionsClick");
            View.AddCallback("CreditsClick");
            View.AddCallback("QuitClick");
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.InMainMenu;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_UP) == TVButtonState.Pressed ||
                JoyStick.JoyUpKeyDown() || Gamepad[0].DPad.Up.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyUp()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_DOWN) == TVButtonState.Pressed ||
                JoyStick.JoyDownKeyDown() || Gamepad[0].DPad.Down.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyDown()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                UIManager.Exit();
            }
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            if (name.Equals("PlayClick"))
            {
                var uiView = UIManager.GetByType(UIType.SelectLevel);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("StatisticsClick"))
            {
                var uiView = UIManager.GetByType(UIType.Statistics);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("OptionsClick"))
            {
                var uiView = UIManager.GetByType(UIType.Options);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("CreditsClick"))
            {
                var uiView = UIManager.GetByType(UIType.Credits);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("QuitClick"))
            {
                Quit();
            }
        }

        public override void PageLoaded()
        {
            System.Threading.Thread.Sleep(200);
        }

        private void Quit()
        {
            core.Exit();
        }
    }

    public class Scene
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool IsTutorial { get; set; }
    }

    public class UISelectLevel : UIView
    {
        List<Scene> scenesList;

        public UISelectLevel(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            View.AddCallback("MainMenuClick");
            View.AddCallback("PlayClick");
            string scenesListFile = Path.Combine(Application.StartupPath, Helpers.GameSettings.ScenesListFile);
            View.SetProperty("scenesFile", new JSValue(scenesListFile));

            // Collect available scenes.
            GetAvailableScenes(scenesListFile);
        }

        private void GetAvailableScenes(string scenesListFile)
        {
            var doc = XDocument.Load(scenesListFile);
            var query = from e in doc.Descendants("Scene")
                        select new
                        {
                            Id = int.Parse(e.Attribute("Id").Value, CultureInfo.InvariantCulture),
                            FileName = e.Attribute("File").Value,
                            IsTutorial = e.Attribute("IsTutorial") == null ? false : bool.Parse(e.Attribute("IsTutorial").Value)
                        };

            scenesList = new List<Scene>();
            foreach (var e in query)
            {
                scenesList.Add(new Scene()
                {
                    Id = e.Id,
                    FileName = e.FileName,
                    IsTutorial = e.IsTutorial
                });
            }
        }

        public override void PageLoaded()
        {
            View.ExecuteJavascript(string.Format(CultureInfo.InvariantCulture, "selectLastPlayed({0})", Helpers.GameSettings.LastPlayedId));
            View.ExecuteJavascript(string.Format(CultureInfo.InvariantCulture, "setMaxPlayed({0})", Helpers.GameSettings.MaxPlayedId));
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            if (name.Equals("MainMenuClick"))
            {
                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("PlayClick"))
            {
                int lastPlayedId = 0;
                int.TryParse(args[0].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out lastPlayedId);
                // Workaround when user too quickly presses play button.
                if (lastPlayedId == 0)
                    return;
                UIManager.HideActive();
                Helpers.GameState = Helpers.State.Playing;
                Core.Settings.StartupScene = scenesList.First(o => o.Id.Equals(lastPlayedId)).FileName;
                UIManager.SetSceneId(lastPlayedId);
                var isTutorial = scenesList.First(o => o.Id.Equals(lastPlayedId)).IsTutorial;
                UIManager.SetIsTutorial(isTutorial);
                Core.GetService<IGameManagerService>().Reload();
                Helpers.GameSettings.LastPlayedId = lastPlayedId;
                if (Helpers.GameSettings.MaxPlayedId < lastPlayedId)
                {
                    Helpers.GameSettings.MaxPlayedId = lastPlayedId;
                }
                Helpers.SaveSettings();

                if (scenesList.Last().Id.Equals(lastPlayedId))
                    UIManager.SetIsLastScene(true);
                else
                    UIManager.SetIsLastScene(false);

                ((UIInGame)UIManager.GetByType(UIType.InGame)).Restart();
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.InSelectLevelMenu;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_LEFT) == TVButtonState.Pressed ||
                JoyStick.JoyLeftKeyDown() || Gamepad[0].DPad.Left.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyLeft()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RIGHT) == TVButtonState.Pressed ||
                JoyStick.JoyRightKeyDown() || Gamepad[0].DPad.Right.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyRight()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_UP) == TVButtonState.Pressed ||
                JoyStick.JoyUpKeyDown() || Gamepad[0].DPad.Up.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyUp()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_DOWN) == TVButtonState.Pressed ||
                JoyStick.JoyDownKeyDown() || Gamepad[0].DPad.Down.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyDown()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }
    }

    public class UILevelFinished : UIView
    {
        List<Scene> scenesList;
        bool resultSaved;

        public UILevelFinished(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            View.AddCallback("MainMenuClick");
            View.AddCallback("NextLevelClick");

            string scenesListFile = Path.Combine(Application.StartupPath, Helpers.GameSettings.ScenesListFile);

            // Collect available scenes.
            GetAvailableScenes(scenesListFile);
        }

        private void GetAvailableScenes(string scenesListFile)
        {
            var doc = XDocument.Load(scenesListFile);
            var query = from e in doc.Descendants("Scene")
                        select new
                        {
                            Id = int.Parse(e.Attribute("Id").Value, CultureInfo.InvariantCulture),
                            FileName = e.Attribute("File").Value,
                            IsTutorial = e.Attribute("IsTutorial") == null ? false : bool.Parse(e.Attribute("IsTutorial").Value)
                        };

            scenesList = new List<Scene>();
            foreach (var e in query)
            {
                scenesList.Add(new Scene()
                {
                    Id = e.Id,
                    FileName = e.FileName,
                    IsTutorial = e.IsTutorial
                });
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.Win;

            if (!resultSaved)
                SaveResult();

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_LEFT) == TVButtonState.Pressed ||
                JoyStick.JoyLeftKeyDown() || Gamepad[0].DPad.Left.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyLeft()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RIGHT) == TVButtonState.Pressed ||
                JoyStick.JoyRightKeyDown() || Gamepad[0].DPad.Right.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyRight()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }

        private void SaveResult()
        {
            var sceneId = Helpers.GameSettings.LastPlayedId;
            var time = ((UIInGame)UIManager.GetByType(UIType.InGame)).GetTime();
            var moves = ((UIInGame)UIManager.GetByType(UIType.InGame)).GetMoves();

            Helpers.GameStatistics.RecordList.Add(new Record()
            {
                Date = DateTime.Now,
                SceneId = sceneId,
                Move = moves,
                Time = time.ToString()
            });
            Helpers.SaveStatistics();

            resultSaved = true;
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            resultSaved = false;

            if (name.Equals("MainMenuClick"))
            {
                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("NextLevelClick"))
            {
                int lastPlayedId = Helpers.GameSettings.LastPlayedId + 1;
                UIManager.HideActive();
                Helpers.GameState = Helpers.State.Playing;
                Core.Settings.StartupScene = scenesList.First(o => o.Id.Equals(lastPlayedId)).FileName;
                UIManager.SetSceneId(lastPlayedId);
                var isTutorial = scenesList.First(o => o.Id.Equals(lastPlayedId)).IsTutorial;
                UIManager.SetIsTutorial(scenesList.First(o => o.Id.Equals(lastPlayedId)).IsTutorial);
                Core.GetService<IGameManagerService>().Reload();
                Helpers.GameSettings.LastPlayedId = lastPlayedId;
                if (Helpers.GameSettings.MaxPlayedId < Helpers.GameSettings.LastPlayedId)
                {
                    Helpers.GameSettings.MaxPlayedId = Helpers.GameSettings.LastPlayedId;
                }
                Helpers.SaveSettings();

                if (scenesList.Last().Id.Equals(lastPlayedId))
                    UIManager.SetIsLastScene(true);
                else
                    UIManager.SetIsLastScene(false);

                ((UIInGame)UIManager.GetByType(UIType.InGame)).Restart();
            }
        }
    }

    public class UIPaused : UIView
    {
        public UIPaused(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            View.AddCallback("MainMenuClick");
            View.AddCallback("RestartClick");
            View.AddCallback("ResumeClick");
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            if (name.Equals("MainMenuClick"))
            {
                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("RestartClick"))
            {
                UIManager.HideActive();
                Helpers.GameState = Helpers.State.Playing;
                Core.GetService<IGameManagerService>().Reload();
                ((UIInGame)UIManager.GetByType(UIType.InGame)).Restart();
            }
            else if (name.Equals("ResumeClick"))
            {
                UIManager.HideActive();
                Helpers.GameState = Helpers.PreviousGameState;
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.Paused;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_LEFT) == TVButtonState.Pressed ||
                JoyStick.JoyLeftKeyDown() || Gamepad[0].DPad.Left.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyLeft()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RIGHT) == TVButtonState.Pressed ||
                JoyStick.JoyRightKeyDown() || Gamepad[0].DPad.Right.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyRight()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }
    }

    public class UIDead : UIView
    {
        public UIDead(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            View.AddCallback("MainMenuClick");
            View.AddCallback("RestartClick");
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            if (name.Equals("MainMenuClick"))
            {
                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("RestartClick"))
            {
                UIManager.HideActive();
                Helpers.GameState = Helpers.State.Playing;
                Core.GetService<IGameManagerService>().Reload();
                ((UIInGame)UIManager.GetByType(UIType.InGame)).Restart();
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.Dead;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_LEFT) == TVButtonState.Pressed ||
                JoyStick.JoyLeftKeyDown() || Gamepad[0].DPad.Left.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyLeft()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RIGHT) == TVButtonState.Pressed ||
                JoyStick.JoyRightKeyDown() || Gamepad[0].DPad.Right.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyRight()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }
    }

    public class UIOptions : UIView
    {
        List<TV_MODEFORMAT> videoModes;

        public UIOptions(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            videoModes = Helpers.GetAvailableVideoModes();

            View.AddCallback("MainMenuClick");
            View.AddCallback("ApplyClick");
            View.AddCallback("MusicVolumeChanged");
            View.AddCallback("FxVolumeChanged");
            View.SetProperty("antialiasing", new JSValue(Helpers.GameSettings.Antialiasing));
            View.SetProperty("musicVolume", new JSValue(Helpers.GameSettings.MusicVolume));
            View.SetProperty("fxVolume", new JSValue(Helpers.GameSettings.FXVolume));
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            if (name.Equals("MainMenuClick"))
            {
                base.OnCallback(name, args);

                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("ApplyClick"))
            {
                base.OnCallback(name, args);

                // Get is full screen.
                FutureJSValue result = View.ExecuteJavascriptWithResult("getIsFullScreen()");
                Helpers.GameSettings.FullScreen = result.Get().ToBoolean();

                // Get video mode.
                result = View.ExecuteJavascriptWithResult("getVideoMode()");
                var idx = 0;
                int.TryParse(result.Get().ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out idx);
                Helpers.GameSettings.ScreenMode = videoModes[idx];

                // Get antialiasing value.
                result = View.ExecuteJavascriptWithResult("getAntialiasingValue()");
                var value = 0;
                int.TryParse(result.Get().ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                Helpers.GameSettings.Antialiasing = value;
                View.SetProperty("antialiasing", new JSValue(Helpers.GameSettings.Antialiasing));

                // Get music volume.
                result = View.ExecuteJavascriptWithResult("getMusicVolume()");
                var musicVolume = 0;
                int.TryParse(result.Get().ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out musicVolume);
                Helpers.GameSettings.MusicVolume = musicVolume;

                // Get fx volume.
                result = View.ExecuteJavascriptWithResult("getFXVolume()");
                var fxVolume = 0;
                int.TryParse(result.Get().ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out fxVolume);
                Helpers.GameSettings.FXVolume = fxVolume;

                // Save settings.
                Helpers.SaveSettings();

                // Apply video changes.
                Core.Settings.ScreenMode = videoModes[idx];
                Core.Settings.MultisampleType = Helpers.GetMultisample(Helpers.GameSettings.Antialiasing);

                if (Helpers.GameSettings.FullScreen)
                {
                    Core.SwitchFullscreen();
                }
                else
                {
                    Core.SwitchWindowed();
                }

                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
            else if (name.Equals("MusicVolumeChanged"))
            {
                var musicVolume = 0;
                int.TryParse(args[0].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out musicVolume);
                Helpers.GameSettings.MusicVolume = musicVolume;
                ApplySoundChanges();
            }
            else if (name.Equals("FxVolumeChanged"))
            {
                var fxVolume = 0;
                int.TryParse(args[0].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out fxVolume);
                Helpers.GameSettings.FXVolume = fxVolume;
                ApplySoundChanges();
            }
        }

        private void ApplySoundChanges()
        {
            var soundManager = Core.GetService<ISoundManagerService>();
            var sceneManager = Core.GetService<ISceneManagerService>();

            // Apply sound volume changes.
            soundManager.SetMusicVolume(Helpers.GameSettings.MusicVolume / 100f);

            // Apply fx volume changes.
            var allFxSounds = sceneManager.GetGameObjects<Sound>().FindAll(o => o.IsFromScript == false);
            foreach (var s in allFxSounds)
            {
                soundManager.SetVolume(s, Helpers.GameSettings.FXVolume / 100f);
            }

            Helpers.SaveSettings();
        }

        public override void PageLoaded()
        {
            // Set is full screen.
            View.ExecuteJavascript(string.Format(CultureInfo.InvariantCulture, "setIsFullScreen({0})", Helpers.GameSettings.FullScreen.ToString().ToLower(CultureInfo.InvariantCulture)));

            // Set current video mode.
            var idx = 0;
            AddVideoModes(videoModes);

            foreach (var videoMode in videoModes)
            {
                if (videoMode.Width.Equals(Helpers.GameSettings.ScreenMode.Width) &&
                    videoMode.Height.Equals(Helpers.GameSettings.ScreenMode.Height))
                {
                    break;
                }

                idx++;
            }
            View.ExecuteJavascript(string.Format(CultureInfo.InvariantCulture, "setVideoMode({0})", idx.ToString()));

            // Set music volume
            View.ExecuteJavascript(string.Format(CultureInfo.InvariantCulture, "setMusicVolume({0})", Helpers.GameSettings.MusicVolume.ToString()));

            // Set fx volume
            View.ExecuteJavascript(string.Format(CultureInfo.InvariantCulture, "setFXVolume({0})", Helpers.GameSettings.FXVolume.ToString()));


            System.Threading.Thread.Sleep(100);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.InOptionsMenu;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_LEFT) == TVButtonState.Pressed ||
                JoyStick.JoyLeftKeyDown() || Gamepad[0].DPad.Left.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyLeft()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RIGHT) == TVButtonState.Pressed ||
                JoyStick.JoyRightKeyDown() || Gamepad[0].DPad.Right.State == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyRight()");
                PlayButtonFocusSound();
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }

        void AddVideoModes(List<TV_MODEFORMAT> modes)
        {
            int idx = 0;

            foreach (TV_MODEFORMAT mode in modes)
            {
                var value = idx++;
                var text = string.Format(CultureInfo.InvariantCulture, "{0} x {1}", mode.Width, mode.Height);
                View.ExecuteJavascript(string.Format(CultureInfo.InvariantCulture, "addVideoMode('{0}', '{1}')", value, text));
            }
        }
    }

    public class UIGameFinished : UIView
    {
        public UIGameFinished(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            View.AddCallback("MainMenuClick");
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.GameFinished;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }

        public override void PageLoaded()
        {
            if (Helpers.GameSettings.MaxPlayedId < Helpers.GameSettings.LastPlayedId)
            {
                Helpers.GameSettings.MaxPlayedId = Helpers.GameSettings.LastPlayedId;
                Helpers.SaveSettings();
            }
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            if (name.Equals("MainMenuClick"))
            {
                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
        }
    }

    public class UICredits : UIView
    {
        public UICredits(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            View.AddCallback("MainMenuClick");
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.GameFinished;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            if (name.Equals("MainMenuClick"))
            {
                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
        }
    }

    public class UIStatistics : UIView
    {
        public UIStatistics(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            View.AddCallback("MainMenuClick");
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.GameFinished;

            if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_RETURN) == TVButtonState.Pressed ||
                JoyStick.JoyButtonDown(0))
            {
                View.ExecuteJavascript("keyEnter()");
            }
            else if (Keyboard.GetKeyState(CONST_TV_KEY.TV_KEY_ESCAPE) == TVButtonState.Pressed)
            {
                View.ExecuteJavascript("keyEscape()");
            }
        }

        public override void OnCallback(string name, JSValue[] args)
        {
            base.OnCallback(name, args);

            if (name.Equals("MainMenuClick"))
            {
                var uiView = UIManager.GetByType(UIType.Main);
                UIManager.SetView(uiView);
            }
        }
    }

    public class UITutorial : UIView
    {
        public UITutorial(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            Helpers.GameState = Helpers.State.Tutorial;
        }

        public override void Reload()
        {
            View.SetProperty("sceneId", new JSValue(UIManager.GetSceneId()));
            base.Reload();
        }
    }

    public class UIInGame : UIView
    {
        public UIInGame(ICore core, UIType menuType, int width, int height, UIFlags flags, bool transparent)
            : base(core, menuType, width, height, flags, transparent)
        {
            var service = Core.GetService<IScriptManagerService>();
            service.SetGlobal("UIInGame", this);
            service.RegisterCustomFunctions(this);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);

            if (Helpers.GameState != Helpers.State.Playing)
            {
                View.ExecuteJavascript("stop()");
            }
        }

        public void Restart()
        {
            View.ExecuteJavascript("restart()");
        }

        public TimeSpan GetTime()
        {
            FutureJSValue result = View.ExecuteJavascriptWithResult("getMinutes()");
            var minutes = 0;
            int.TryParse(result.Get().ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out minutes);

            result = View.ExecuteJavascriptWithResult("getSeconds()");
            var seconds = 0;
            int.TryParse(result.Get().ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out seconds);

            return new TimeSpan(0, minutes, seconds);
        }

        public int GetMoves()
        {
            FutureJSValue result = View.ExecuteJavascriptWithResult("getMoves()");
            var moves = 0;
            int.TryParse(result.Get().ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out moves);

            return moves;
        }

        [RegisterFunction]
        public void DoStep()
        {
            View.ExecuteJavascript("start()");
            View.ExecuteJavascript("rotateCube()");
        }
    }
}
