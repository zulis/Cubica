using System;
using System.Collections.Generic;
using ComponentFramework.Core;
using IrrKlang;
using MTV3D65;
using Cubica.Components.Objects;

namespace Cubica.Managers
{
    [AutoLoad]
    partial class SoundManager : Component, ISoundManagerService
    {
        ISoundEngine soundEngine;

        public SoundManager(ICore core) : base(core) { Order = int.MinValue + 10; }

        public override void Initialize()
        {
            soundEngine = new ISoundEngine();

            // Register object in Lua.
            ScriptManager.SetGlobal("Sound", this);
        }

        public override void Dispose()
        {
            soundEngine.StopAllSounds();
            soundEngine.RemoveAllSoundSources();
            soundEngine = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            var camera = Core.Engine.GetCamera();
            var position = camera.GetPosition();
            var lookDir = camera.GetLookAt() - camera.GetPosition();
            soundEngine.SetListenerPosition(new Vector3D(position.x, position.y, position.z),
                                            new Vector3D(lookDir.x, lookDir.y, lookDir.z).Normalize());
            soundEngine.Update();
        }

        public void Load(Sound sound)
        {
            sound.iSourceSource = soundEngine.GetSoundSource(sound.FileName, true);

            switch (sound.Is3D)
            {
                case true:
                    sound.iSound = soundEngine.Play3D(sound.iSourceSource, sound.Position.x, sound.Position.y, sound.Position.z, sound.Loop, sound.Stopped, false);
                    break;
                default:
                    sound.iSound = soundEngine.Play2D(sound.iSourceSource, sound.Loop, sound.Stopped, false);
                    break;
            }

            StopAllSounds();
        }

        public void SetPosition(Sound sound, float x, float y, float z)
        {
            sound.Position = new TV_3DVECTOR(x, y, z);
        }

        public void StopAllSounds()
        {
            var allSounds = SceneManager.GetGameObjects<Sound>().FindAll(o=>o.IsFromScript == false);

            foreach (var s in allSounds)
            {
                s.Stop();
            }
        }

        public void RemoveAllSounds()
        {
            StopAllSounds();

            var allSounds = SceneManager.GetGameObjects<Sound>().FindAll(o => o.IsFromScript == false);
            var soundsToRemove = new List<Sound>();

            foreach (var s in allSounds)
            {
                s.iSound.Dispose();
                soundsToRemove.Add(s);
            }

            foreach (Sound s in soundsToRemove.ToArray())
            {
                Core.UnloadComponent(s);
            }
            soundsToRemove.Clear();
        }

        public void Play(Sound sound)
        {
            switch (sound.Is3D)
            {
                case true:
                    sound.iSound = soundEngine.Play3D(sound.iSourceSource, sound.Position.x, sound.Position.y, sound.Position.z, sound.Loop, false, false);
                    break;
                default:
                    sound.iSound = soundEngine.Play2D(sound.iSourceSource, sound.Loop, false, false);
                    break;
            }
        }

        public void Stop(Sound sound)
        {
            sound.iSound.Stop();
        }

        public void SetVolume(Sound sound)
        {
            sound.iSourceSource.DefaultVolume = sound.Volume;
            sound.iSound.Volume = sound.Volume;
        }

        public void SetVolume(Sound sound, float value)
        {
            sound.Volume = value;
            SetVolume(sound);
        }

        [ServiceDependency]
        public new IScriptManagerService ScriptManager { private get; set; }
        [ServiceDependency]
        public new ISceneManagerService SceneManager { private get; set; }
    }

    interface ISoundManagerService : IService
    {
        void Load(Sound sound);
        void Play(Sound sound);
        void Stop(Sound sound);
        void SetVolume(Sound sound);
        void SetVolume(Sound sound, float value);
        void SetMusicVolume(float value);
        void SetPosition(Sound sound, float x, float y, float z);
        void StopAllSounds();
        void RemoveAllSounds();
        Sound Load2DSound(string fileName);
        void Play2DSound(Sound sound);
    }
}
