using MTV3D65;
using Cubica.Managers;

namespace Cubica.Components.Objects
{
    partial class PointLight
    {
        [RegisterFunction]
        public void Enable()
        {
            LightEngine.EnableLight(LightId, true);
        }

        [RegisterFunction]
        public void Disable()
        {
            LightEngine.EnableLight(LightId, false);
        }

        [RegisterFunction]
        public void SetRadius(float radius)
        {
            LightEngine.SetLightRange(LightId, radius);
        }

        [RegisterFunction]
        public void SetPosition(TV_3DVECTOR position)
        {
            SetPosition(position.x, position.y, position.z);
        }

        [RegisterFunction]
        public void SetPosition(float x, float y, float z)
        {
            Position = new TV_3DVECTOR(x, y, z);
            LightEngine.SetLightPosition(LightId, x, y, z);
        }

        [RegisterFunction]
        public TV_3DVECTOR GetPosition()
        {
            return Position;
        }

        [RegisterFunction]
        public void SetColor(float r, float g, float b)
        {
            LightEngine.SetLightColor(LightId, r / 255f, g / 255f, b / 255f);
        }
    }
}
