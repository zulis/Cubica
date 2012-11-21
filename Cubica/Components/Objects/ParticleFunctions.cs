using MTV3D65;
using Cubica.Managers;
using System.Collections.Generic;

namespace Cubica.Components.Objects
{
    partial class Particle
    {
        [RegisterFunction]
        public void Enable()
        {
            particle.Enable(true);
        }

        [RegisterFunction]
        public void Disable()
        {
            particle.Enable(false);
        }

        [RegisterFunction]
        public void Move(TV_3DVECTOR target)
        {
            Move(target.x, target.y, target.z);
        }

        [RegisterFunction]
        public void Stop()
        {
            for (int i = 0; i < particle.GetEmitterCount(); i++)
                particle.SetEmitterLooping(i, false);
        }

        [RegisterFunction]
        public void Move(float x, float y, float z)
        {
            particle.SetGlobalPosition(x, y, z);
        }

        [RegisterFunction]
        public void AttachTo(ObjectBase target)
        {
            var obj = SceneManager.GetGameObjects().FindLast(o=>o.Equals(target)) as ObjectBase;
            Move(obj.Position);
        }
    }
}
