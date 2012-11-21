using System;
using Cubica.Components.Objects;

namespace Cubica.Managers
{
    partial class SoundManager
    {
        Sound music;

        [RegisterFunction]
        public void PlayMusic(string fileName)
        {
            PlayMusic(fileName, false);
        }

        [RegisterFunction]
        public void PlayMusic(string fileName, bool loop)
        {
            if (!IsMusicPlaying() && music != null)
            {
                Stop(music);
            }

            music = new Sound(Core)
            {
                Name = Guid.NewGuid().ToString(),
                FileName = fileName,
                Volume = Helpers.GameSettings.MusicVolume / 100f,
                Stopped = false,
                Loop = loop,
                Is3D = false,
                ScriptEnabled = false,
                IsFromScript = true
            };

            Core.LoadComponent<Sound>(music);
        }

        [RegisterFunction]
        public void StopMusic()
        {
            if (music != null)
            {
                Core.UnloadComponent(music);
                music = null;
            }
        }

        [RegisterFunction]
        public bool IsMusicPlaying()
        {
            if (music != null)
            {
                return !music.iSound.Finished;
            }
            else
                return false;
        }

        [RegisterFunction]
        public void SetMusicVolume(float value)
        {
            if (music != null)
            {
                music.Volume = value;
                SetVolume(music);
            }
        }

        [RegisterFunction]
        public Sound Load2DSound(string fileName)
        {
            var sound = new Sound(Core)
            {
                Name = Guid.NewGuid().ToString(),
                FileName = fileName,
                Volume = 1.0f,
                Stopped = true,
                Loop = false,
                Is3D = false,
                ScriptEnabled = false,
                IsFromScript = true
            };

            Core.LoadComponent<Sound>(sound);

            return sound;
        }

        [RegisterFunction]
        public void Play2DSound(Sound sound)
        {
            Play(sound);
        }
    }
}
