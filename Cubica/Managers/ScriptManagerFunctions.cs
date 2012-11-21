using MTV3D65;

namespace Cubica.Managers
{
    partial class ScriptManager
    {
        [RegisterFunction]
        public void dbg(string name, object item)
        {
            DebuggingBag.Put(name, item);
        }

        [RegisterFunction]
        public TV_2DVECTOR Vector2d(float x, float y)
        {
            return new TV_2DVECTOR(x, y);
        }

        [RegisterFunction]
        public TV_3DVECTOR Vector3d(float x, float y, float z)
        {
            return new TV_3DVECTOR(x, y, z);
        }
    }
}
