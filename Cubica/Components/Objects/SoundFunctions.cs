using MTV3D65;
using Cubica.Managers;
using System.Threading;

namespace Cubica.Components.Objects
{
    partial class Sound
    {
        [RegisterFunction]
        public void Play()
        {
            SoundManager.Play(this);
        }

        [RegisterFunction]
        public void Play(int pause)
        {
            Thread t = new Thread(delegate()
                {
                    Thread.Sleep(pause);
                    Play();
                });
            t.Start();
        }

        [RegisterFunction]
        public void Stop()
        {
            SoundManager.Stop(this);
        }

        [RegisterFunction]
        public void SetPosition(TV_3DVECTOR target)
        {
            SoundManager.SetPosition(this, target.x, target.y, target.z);
        }

        [RegisterFunction]
        public void SetPosition(float x, float y, float z)
        {
            SoundManager.SetPosition(this, x, y, z);
        }

        [RegisterFunction]
        public void SetVolume(float value)
        {
            SoundManager.SetVolume(this, value);
        }
    }
}
