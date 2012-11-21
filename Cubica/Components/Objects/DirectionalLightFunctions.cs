using MTV3D65;
using Cubica.Managers;

namespace Cubica.Components.Objects
{
    /// <summary>
    /// Directional light class.
    /// </summary>
    partial class DirectionalLight
    {
        /// <summary>
        /// Enables light.
        /// </summary>
        [RegisterFunction]
        public void Enable()
        {
            LightEngine.EnableLight(LightId, true);
        }

        /// <summary>
        /// Disables light.
        /// </summary>
        [RegisterFunction]
        public void Disable()
        {
            LightEngine.EnableLight(LightId, false);
        }

        /// <summary>
        /// Sets the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        [RegisterFunction]
        public void SetDirection(TV_3DVECTOR direction)
        {
            SetDirection(direction.x, direction.y, direction.z);
        }

        /// <summary>
        /// Sets the direction.
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        [RegisterFunction]
        public void SetDirection(float x, float y, float z)
        {
            LightEngine.SetLightDirection(LightId, x, y, z);
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        [RegisterFunction]
        public void SetColor(float r, float g, float b)
        {
            LightEngine.SetLightColor(LightId, r / 255f, g / 255f, b / 255f);
        }
    }
}
