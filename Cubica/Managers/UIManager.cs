using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AwesomiumDotNet;
using ComponentFramework.Core;

namespace Cubica.Managers
{
    [AutoLoad]
    partial class UIManager : Component, IUIManagerService
    {
        WebCore webCore;
        Dictionary<UIView, bool> gameMenu;
        bool cursorVisible;
        bool isLastScene;
        int sceneId;
        bool isTutorial;
        UIView mainMenu, selectLevel, levelFinished, paused, dead, options,
            gameFinished, credits, statistics, tutorial, inGame;

        public UIManager(ICore core) : base(core) { Order = int.MinValue + 11; }

        public override void Initialize()
        {
            // Register object in Lua.
            ScriptManager.SetGlobal("Menu", this);
            ScriptManager.SetGlobal("isLastScene", false);

            webCore = new WebCore(LogLevel.None);
            //menuList = new List<UIView>();
            gameMenu = new Dictionary<UIView, bool>();

            // Load main menu
            mainMenu = new UIMain(Core, UIType.Main, 512, 512, UIFlags.Center, true);
            mainMenu.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/main.html"));
            gameMenu.Add(mainMenu, false);

            // Load select level menu
            selectLevel = new UISelectLevel(Core, UIType.SelectLevel, 512, 512, UIFlags.Center, true);
            selectLevel.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/select_level.html"));
            gameMenu.Add(selectLevel, false);

            // Load level finished menu
            levelFinished = new UILevelFinished(Core, UIType.Win, 512, 512, UIFlags.Center, true);
            levelFinished.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/level_finished.html"));
            gameMenu.Add(levelFinished, false);

            // Load pause menu
            paused = new UIPaused(Core, UIType.Pause, 512, 512, UIFlags.Center, true);
            paused.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/paused.html"));
            gameMenu.Add(paused, false);

            // Load dead menu
            dead = new UIDead(Core, UIType.Dead, 512, 512, UIFlags.Center, true);
            dead.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/dead.html"));
            gameMenu.Add(dead, false);

            // Load options menu
            options = new UIOptions(Core, UIType.Options, 512, 512, UIFlags.Center, true);
            options.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/options.html"));
            gameMenu.Add(options, false);

            // Load game finished menu
            gameFinished = new UIGameFinished(Core, UIType.Finished, 512, 512, UIFlags.Center, true);
            gameFinished.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/game_finished.html"));
            gameMenu.Add(gameFinished, false);

            // Load credits menu
            credits = new UICredits(Core, UIType.Credits, 512, 512, UIFlags.Center, true);
            credits.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/credits.html"));
            gameMenu.Add(credits, false);

            // Load statistics menu
            statistics = new UIStatistics(Core, UIType.Statistics, 512, 512, UIFlags.Center, true);
            statistics.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/statistics.html"));
            gameMenu.Add(statistics, false);

            // Load tutorial menu
            tutorial = new UITutorial(Core, UIType.Tutorial, 512, 512, UIFlags.BottomCenter, true);
            tutorial.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/tutorial.html"));
            gameMenu.Add(tutorial, false);

            // Load inGame menu
            inGame = new UIInGame(Core, UIType.InGame, 512, 512, UIFlags.BottomRight, true);
            inGame.LoadFile(Path.Combine(Application.StartupPath, @"data/ui/ingame.html"));
            gameMenu.Add(inGame, false);

            Helpers.GameState = Helpers.State.InMainMenu;

            // Set active menu only if we didn't came from an editor.
            if (!Core.Settings.PreviewMode)
            {
                gameMenu[mainMenu] = true;
            }
        }

        public override void Dispose()
        {
            foreach (var gm in gameMenu)
            {
                gm.Key.Dispose();
            }

            gameMenu.Clear();
            webCore.Dispose();
            webCore = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (Helpers.GameState == Helpers.State.Playing &&
                gameMenu[inGame] == false)
            {
                gameMenu[inGame] = true;
                inGame.Reload();
            }
            else if (Helpers.GameState == Helpers.State.Tutorial)
            {
                gameMenu[inGame] = false;
            }

            foreach (var gm in gameMenu)
            {
                if (gm.Value == true)
                    gm.Key.Update(elapsedTime);
            }

            // Since quiting from application is possible
            // from active UI, we have to check if webCore
            // is null or not.
            if (webCore != null)
            {
                webCore.Update();
            }
            else
            {
                return;
            }

            cursorVisible = gameMenu[options] == true;
            Core.Engine.ShowWinCursor(cursorVisible);
        }

        public override void PostDraw()
        {
            foreach (var gm in gameMenu)
            {
                if (gm.Value == true)
                    gm.Key.PostDraw();
            }
        }

        public WebCore GetWebCore()
        {
            return webCore;
        }

        public void SetView(UIView uiView)
        {
            HideActive();
            gameMenu[uiView] = true;
            uiView.Reload();
        }

        public UIView GetByType(UIType type)
        {
            foreach (var gm in gameMenu)
            {
                if (gm.Key.MenuType.Equals(type))
                    return gm.Key;
            }

            return null;
        }

        public bool IsCursorVisible()
        {
            return cursorVisible;
        }

        public void Exit()
        {
            HideActive();
            Core.Exit();
        }

        public void SetIsLastScene(bool value)
        {
            isLastScene = value;
        }

        public void SetSceneId(int value)
        {
            sceneId = value;
        }

        public void SetIsTutorial(bool value)
        {
            isTutorial = value;
        }

        [ServiceDependency]
        public new IScriptManagerService ScriptManager { private get; set; }
    }

    public interface IUIManagerService : IService
    {
        WebCore GetWebCore();
        void SetView(UIView uiView);
        void HideActive();
        UIView GetByType(UIType type);
        bool IsCursorVisible();
        void Exit();
        void SetIsLastScene(bool value);
        void SetSceneId(int value);
        int GetSceneId();
        void SetIsTutorial(bool value);
    }
}