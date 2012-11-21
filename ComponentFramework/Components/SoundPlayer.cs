using System.IO;
using System.Windows.Forms;
using ComponentFramework.Core;
using IrrKlang;

namespace ComponentFramework.Components
{
    public class SoundPlayer : Component, ISoundPlayerService
    {
        ISoundEngine engine;
        bool paused;

        public SoundPlayer(ICore core) : base(core)
        {
            Order = int.MinValue;
        }

        public override void Initialize()
        {
            engine = new ISoundEngine();
        }

        public ISoundSource Load(string filename)
        {
            return engine.GetSoundSource(Path.Combine(Application.StartupPath, Path.Combine(Core.ContentRoot, filename)), true);
        }

        public ISound Play(ISoundSource source)
        {
            return Play(source, false);
        }
        public ISound Play(ISoundSource source, bool loop)
        {
            return GetSound(source, loop, true);
        }
        public ISound GetSound(ISoundSource source, bool loop, bool play)
        {
            return engine.Play2D(source, loop, !play, false);
        }

        public bool Paused
        {
            get { return paused; }
            set 
            {
                paused = value;
                engine.SetAllSoundsPaused(value); 
            }
        }

        public float Volume
        {
            get { return engine.SoundVolume; }
            set { engine.SoundVolume = value; }
        }

        protected override void DisposeInternal()
        {
            engine.StopAllSounds();
            engine.RemoveAllSoundSources();
        }
    }

    public interface ISoundPlayerService : IService
    {
        ISoundSource Load(string filename);

        ISound Play(ISoundSource source);
        ISound Play(ISoundSource source, bool loop);
        ISound GetSound(ISoundSource source, bool loop, bool play);

        float Volume { get; set; }
        bool Paused { get; set; }
    }
}
