using System;
using MTV3D65;
using System.Windows.Forms;

namespace Cubica
{
    [Serializable]
    public class Settings
    {
        public Settings()
        {
            ScenesListFile = @"data/scenes/scenes.list";
            LastPlayedId = 0;
            MaxPlayedId = 1;
            MinimapWidth = 300;
            MinimapHeight = 100;
            FullScreen = false;
            ScreenMode = new TV_MODEFORMAT() { Width = 800, Height = 600, Format = 32 };
#if DEBUG
            //FullScreen = false;
            //ScreenMode = new TV_MODEFORMAT() { Width = 800, Height = 600, Format = 32 };
            Antialiasing = 2;

#else
            //FullScreen = true;
            //ScreenMode = new TV_MODEFORMAT() { 
            //    Width = Screen.PrimaryScreen.Bounds.Width,
            //    Height = Screen.PrimaryScreen.Bounds.Height,
            //    Format = 32
            //};
            Antialiasing = 4;
#endif
            MusicVolume = 70;
            FXVolume = 80;
        }

        public string ScenesListFile { get; set; }
        public int LastPlayedId { get; set; }
        public int MaxPlayedId { get; set; }
        public int MinimapWidth { get; set; }
        public int MinimapHeight { get; set; }
        public bool FullScreen { get; set; }
        public TV_MODEFORMAT ScreenMode;
        public int MusicVolume { get; set; }
        public int FXVolume { get; set; }
        public int Antialiasing { get; set; }
    }
}
