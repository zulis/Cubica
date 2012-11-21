using System.Globalization;
using System.Collections.Generic;

namespace Cubica.Managers
{
    partial class UIManager
    {
        [RegisterFunction]
        public void Show(string menuName)
        {
            HideActive();

            switch (menuName.ToLower(CultureInfo.InvariantCulture).Trim())
            {
                case "main":
                    gameMenu[mainMenu] = true;
                    mainMenu.Reload();
                    break;
                case "win":
                    gameMenu[levelFinished] = true;
                    levelFinished.Reload();
                    break;
                case "pause":
                    gameMenu[paused] = true;
                    paused.Reload();
                    paused.PlayButtonClickSound();
                    break;
                case "dead":
                    gameMenu[dead] = true;
                    dead.Reload();
                    dead.PlayButtonClickSound();
                    break;
                case "finish":
                    gameMenu[gameFinished] = true;
                    gameFinished.Reload();
                    break;
                case "tutorial":
                    gameMenu[tutorial] = true;
                    tutorial.Reload();
                    break;
                default:
                    break;
            }
        }

        [RegisterFunction]
        public void HideActive()
        {
            List<UIView> uiList = new List<UIView>();
            foreach (var gm in gameMenu)
            {
                if (gm.Value == true && !gm.Key.Equals(inGame))
                    uiList.Add(gm.Key);
            }

            foreach (var ui in uiList)
            {
                gameMenu[ui] = false;
            }
        }

        [RegisterFunction]
        public bool IsVisible()
        {
            bool visible = false;

            foreach (var gm in gameMenu)
            {
                if (gm.Value == true && gm.Key != tutorial && gm.Key != inGame)
                {
                    return true;
                }
            }

            return visible;
        }

        [RegisterFunction]
        public Helpers.State GetState()
        {
            return Helpers.GameState;
        }

        [RegisterFunction]
        public bool IsLastScene()
        {
            return isLastScene;
        }

        [RegisterFunction]
        public int GetSceneId()
        {
            return sceneId;
        }

        [RegisterFunction]
        public bool IsTutorial()
        {
            return isTutorial;
        }
    }
}
