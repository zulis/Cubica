using Cubica.Components.Objects;

namespace Cubica.Managers
{
    partial class GameManager
    {
        [RegisterFunction]
        public void LoadLevel(string fileName)
        {
            SceneManager.LoadLevel(fileName);
        }

        [RegisterFunction]
        public void SetTriggersVisible(bool value)
        {
            foreach (var obj in SceneManager.GetGameObjects())
            {
                if (obj.GetType().Equals(typeof(Trigger)))
                {
                    (obj as Trigger).Show = value;
                }
            }
        }
    }
}
