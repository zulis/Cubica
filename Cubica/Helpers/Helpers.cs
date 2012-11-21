using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;
using ComponentFramework.Core;
using Cubica.Components.Objects;
using MTV3D65;

namespace Cubica
{
    static class Helpers
    {
        public enum FileFormat
        {
            X,
            TVM,
            TVA,
            WAV,
            MP3,
            OGG,
            Unknown
        }

        private const string FILE_MP3 = ".MP3";
        private const string FILE_OGG = ".OGG";
        private const string FILE_TVA = ".TVA";
        private const string FILE_TVM = ".TVM";
        private const string FILE_WAV = ".WAV";
        private const string FILE_X = ".X";

        private const string BOUNDING_BOX = "BOX";
        private const string BOUNDING_CONVEXHULL = "CONVEXHULL";
        private const string BOUNDING_CYLINDER = "CYLINDER";
        private const string BOUNDING_NONE = "NONE";
        private const string BOUNDING_SPHERE = "SPHERE";

        /// <summary>
        /// Gets the file format.
        /// </summary>
        public static FileFormat GetFileFormat(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);

            switch (fi.Extension.ToUpperInvariant())
            {
                case FILE_X:
                    return FileFormat.X;
                case FILE_TVM:
                    return FileFormat.TVM;
                case FILE_TVA:
                    return FileFormat.TVA;
                case FILE_WAV:
                    return FileFormat.WAV;
                case FILE_MP3:
                    return FileFormat.MP3;
                case FILE_OGG:
                    return FileFormat.OGG;
                default:
                    return FileFormat.Unknown;
            }
        }

        /// <summary>
        /// Gets TV_3DVECTOR from string.
        /// </summary>
        public static TV_3DVECTOR GetVector3D(string value)
        {
            var result = new TV_3DVECTOR();
            string[] v = value.Split(' ');

            float f = 0f;
            float.TryParse(v[0], NumberStyles.Any, CultureInfo.InvariantCulture, out f);
            result.x = f;

            f = 0f;
            float.TryParse(v[1], NumberStyles.Any, CultureInfo.InvariantCulture, out f);
            result.y = f;

            f = 0f;
            float.TryParse(v[2], NumberStyles.Any, CultureInfo.InvariantCulture, out f);
            result.z = f;

            return result;
        }

        /// <summary>
        /// Gets bool from string.
        /// </summary>
        public static bool GetBool(string value)
        {
            return value.Trim().ToUpperInvariant().Equals("TRUE");
        }

        /// <summary>
        /// Gets bounding type from string.
        /// </summary>
        public static CONST_TV_PHYSICSBODY_BOUNDING GetBoundingType(string bounding)
        {
            switch (bounding.ToUpperInvariant())
            {
                case BOUNDING_BOX:
                    return CONST_TV_PHYSICSBODY_BOUNDING.TV_BODY_BOX;
                case BOUNDING_CONVEXHULL:
                    return CONST_TV_PHYSICSBODY_BOUNDING.TV_BODY_CONVEXHULL;
                case BOUNDING_CYLINDER:
                    return CONST_TV_PHYSICSBODY_BOUNDING.TV_BODY_CYLINDER;
                case BOUNDING_NONE:
                    return CONST_TV_PHYSICSBODY_BOUNDING.TV_BODY_NONE;
                case BOUNDING_SPHERE:
                    return CONST_TV_PHYSICSBODY_BOUNDING.TV_BODY_SPHERE;
                default:
                    return CONST_TV_PHYSICSBODY_BOUNDING.TV_BODY_CONVEXHULL;
            }
        }

        /// <summary>
        /// Gets float from string.
        /// </summary>
        public static float GetFloat(string value)
        {
            float result = 0f;
            float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int GetInt(string value)
        {
            int result = 0;
            int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Returns <see cref="CONST_TV_MULTISAMPLE_TYPE"/> from integer value.
        /// </summary>
        public static CONST_TV_MULTISAMPLE_TYPE GetMultisample(int value)
        {
            switch (value)
            {
                case 2:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_2_SAMPLES;
                case 3:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_3_SAMPLES;
                case 4:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_4_SAMPLES;
                case 5:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_5_SAMPLES;
                case 6:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_6_SAMPLES;
                case 7:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_7_SAMPLES;
                case 8:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_8_SAMPLES;
                case 9:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_9_SAMPLES;
                case 10:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_10_SAMPLES;
                case 11:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_11_SAMPLES;
                case 12:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_12_SAMPLES;
                case 13:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_13_SAMPLES;
                case 14:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_14_SAMPLES;
                case 15:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_15_SAMPLES;
                case 16:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_16_SAMPLES;
                default:
                    return CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_NONE;
            }
        }

        /// <summary>
        /// Dame states enumeration.
        /// </summary>
        public enum State
        {
            InMainMenu,
            InSelectLevelMenu,
            InOptionsMenu,
            Playing,
            Paused,
            Win,
            Dead,
            GameFinished,
            Tutorial
        }

        /// <summary>
        /// Defines game state.
        /// </summary>
        public static State GameState {
            get { return gameState; }
            set
            {
                if (gameState != value)
                    PreviousGameState = gameState;
                gameState = value;
            }
        }
        private static State gameState;

        /// <summary>
        /// Defines previous game state.
        /// </summary>
        public static State PreviousGameState;

        /// <summary>
        /// Game settings.
        /// </summary>
        public static Settings GameSettings = LaodSettings();

        /// <summary>
        /// Saves game settings.
        /// </summary>
        public static void SaveSettings()
        {
            try
            {
                var settingsFile = Path.Combine(Application.StartupPath, Constants.SETTINGS_FILENAME);
                using (var fs = new FileStream(settingsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var xs = new XmlSerializer(typeof(Settings));
                    xs.Serialize(fs, GameSettings);
                    fs.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads game settings.
        /// </summary>
        private static Settings LaodSettings()
        {
            var settingsFile = Path.Combine(Application.StartupPath, Constants.SETTINGS_FILENAME);
            if (File.Exists(settingsFile))
            {
                try
                {
                    using (var fs = new FileStream(settingsFile, FileMode.Open))
                    {
                        var xs = new XmlSerializer(typeof(Settings));
                        var settings = (Settings)xs.Deserialize(fs);
                        fs.Close();
                        return settings;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new Settings();
                }
            }
            else
            {
                return new Settings();
            }
        }

        /// <summary>
        /// Game settings.
        /// </summary>
        public static Statistics GameStatistics = LaodStatistics();

        /// <summary>
        /// Saves game statistics.
        /// </summary>
        public static void SaveStatistics()
        {
            try
            {
                var settingsFile = Path.Combine(Application.StartupPath, Constants.STATISTICS_FILENAME);
                using (var fs = new FileStream(settingsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var xs = new XmlSerializer(typeof(Statistics));
                    xs.Serialize(fs, GameStatistics);
                    fs.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads game statistics.
        /// </summary>
        private static Statistics LaodStatistics()
        {
            var statisticsFile = Path.Combine(Application.StartupPath, Constants.STATISTICS_FILENAME);
            if (File.Exists(statisticsFile))
            {
                try
                {
                    using (var fs = new FileStream(statisticsFile, FileMode.Open))
                    {
                        var xs = new XmlSerializer(typeof(Statistics));
                        var statistics = (Statistics)xs.Deserialize(fs);
                        fs.Close();
                        return statistics;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new Statistics();
                }
            }
            else
            {
                return new Statistics();
            }
        }

        /// <summary>
        /// Gets the available video modes.
        /// </summary>
        /// <returns></returns>
        public static List<TV_MODEFORMAT> GetAvailableVideoModes()
        {
            var modes = new List<TV_MODEFORMAT>();
            var deviceInfo = new TVDeviceInfo();

            for (int i = 0; i < deviceInfo.GetDisplayModeCount(); i++)
            {
                TV_MODEFORMAT mode = deviceInfo.GetDisplayMode(i);
                if (mode.Format == 32 && !modes.Any(m => m.Width == mode.Width && m.Height == mode.Height))
                    modes.Add(mode);
            }
            modes.Sort((a, b) => ((b.Width - a.Width) << 16) + b.Height - a.Height);

            return modes;
        }

        /// <summary>
        /// Creates the mini map.
        /// </summary>
        /// <param name="gameObjects">The game objects.</param>
        /// <returns></returns>
        public static Bitmap CreateMinimap(List<object> gameObjects)
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            List<Mesh> meshList = GetGameObjects<Mesh>(gameObjects);
            meshList.RemoveAll(o => o.GetCustParam("type").Equals("background"));

            foreach (Mesh mesh in meshList)
            {
                var minBB = mesh.GetMinBBox();
                var maxBB = mesh.GetMaxBBox();
                minX = minX > minBB.x ? minBB.x : minX;
                maxX = maxX < maxBB.x ? maxBB.x : maxX;
                minY = minY > minBB.z ? minBB.z : minY;
                maxY = maxY < maxBB.z ? maxBB.z : maxY;
            }

            // Get values for move to positive x,y side.
            var moveX = -minX;
            var moveY = -minY;

            if (minX < 0)
            {
                maxX += Math.Abs(minX);
                minX = 0;
            }

            if (minY < 0)
            {
                maxY += Math.Abs(minY);
                minY = 0;
            }

            // Get how much do we have to scale objects.            
            var widthScale = GameSettings.MinimapWidth / (maxX - minX);
            var heightScale = GameSettings.MinimapHeight / (maxY - minY);
            var scale = Math.Min(widthScale, heightScale);

            // Get values required for move mini map to center.
            var i = (GameSettings.MinimapWidth - (maxX - minX) * scale) / 2;
            var j = (GameSettings.MinimapHeight - (maxY - minY) * scale) / 2;

            var bmp = new Bitmap(GameSettings.MinimapWidth, GameSettings.MinimapHeight, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(bmp);
            g.CompositingMode = CompositingMode.SourceCopy;
            var brush = new SolidBrush(Color.Black);

            foreach (Mesh mesh in meshList)
            {
                var minBB = mesh.GetMinBBox();
                var maxBB = mesh.GetMaxBBox();
                minX = minBB.x;
                minY = minBB.z;
                maxX = maxBB.x;
                maxY = maxBB.z;
                var x = (minX + moveX) * scale + i;
                var y = (minY + moveY) * scale + j;
                var w = (maxX - minX) * scale;
                var h = (maxY - minY) * scale;

                g.FillRectangle(brush, x, y, w, h);
            }

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            brush.Dispose();
            g.Dispose();
            return bmp;
        }

        /// <summary>
        /// Gets the game objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObjects">The game objects.</param>
        /// <returns></returns>
        public static List<T> GetGameObjects<T>(List<object> gameObjects)
        {
            return gameObjects.FindAll(o => o.GetType().Equals(typeof(T))).Cast<T>().ToList();
        }

        /// <summary>
        /// Gets the DUDV texture from resource.
        /// </summary>
        /// <param name="core">The core.</param>
        /// <param name="texture">The texture.</param>
        /// <returns></returns>
        public static int GetDUDVTextureFromResource(ICore core, Bitmap texture)
        {
            var ms = new MemoryStream();
            texture.Save(ms, ImageFormat.Png);
            ms.Seek(0, 0);
            var data = ms.ToArray();
            ms.Dispose();
            texture.Dispose();
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var addr = handle.AddrOfPinnedObject().ToInt32();
            handle.Free();
            var ds = core.Globals.GetDataSourceFromMemory(addr, data.Length - 1);
            return core.TextureFactory.LoadDUDVTexture(ds, texture.ToString(), -1, -1, 25);
        }
    }
}
