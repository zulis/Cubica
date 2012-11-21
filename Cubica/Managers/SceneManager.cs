using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using ComponentFramework.Components;
using ComponentFramework.Core;
using Cubica.Components.Objects;

namespace Cubica.Managers
{
    [AutoLoad]
    partial class SceneManager : Component, ISceneManagerService
    {
        private List<object> gameObjects;
        private bool skySphereExist;
        private bool skyBoxExist;
        private bool waterExists;

        public SceneManager(ICore core) : base(core) 
        {
            Order = int.MinValue + 5;
            gameObjects = new List<object>();
        }

        public override void Initialize()
        {
            // Register object in Lua.
            ScriptManager.SetGlobal("Scene", this);
        }

        public void UnloadLevel()
        {
            CleanScene();
        }

        public void LoadLevel(string fileName)
        {
            // Set fade in effect
            GraphicEffect.FadeIn();
            CleanScene();
            var scenesToLoad = new Dictionary<string, bool>();

            string sceneFile = string.Empty;
            if (File.Exists(fileName))
            {
                sceneFile = fileName;
            }
            else if (File.Exists(Path.Combine(Application.StartupPath, fileName)))
            {
                sceneFile = Path.Combine(Application.StartupPath, fileName);
            }

            // First add scene file since we will generate mini map from it.
            scenesToLoad.Add(sceneFile, true);
            // Add global.xml if it exists.
            var globalScene = Path.Combine(new FileInfo(sceneFile).Directory.FullName, "global.xml");
            if (File.Exists(globalScene))
            {
                scenesToLoad.Add(globalScene, false);
            }

            foreach (var scene in scenesToLoad)
            {
                var xDoc = XDocument.Load(scene.Key);

                #region Mesh
                var meshQuery = from e in xDoc.Descendants("Objects")
                                from i in e.Descendants("Object")
                                where Helpers.GetBool(i.Element("IsAnimated").Value) == false
                                select new
                                {
                                    Name = i.Element("Name").Value,
                                    FileName = i.Element("FileName").Value,
                                    Visible = Helpers.GetBool(i.Element("Visible").Value),
                                    Position = Helpers.GetVector3D(i.Element("Position").Value),
                                    Rotation = Helpers.GetVector3D(i.Element("Rotation").Value),
                                    Scale = Helpers.GetVector3D(i.Element("Scale").Value),
                                    Mass = Helpers.GetFloat(i.Element("Mass").Value),
                                    StaticFriction = Helpers.GetFloat(i.Element("StaticFriction").Value),
                                    KineticFriction = Helpers.GetFloat(i.Element("KineticFriction").Value),
                                    Softness = Helpers.GetFloat(i.Element("Softness").Value),
                                    Bounciness = Helpers.GetFloat(i.Element("Bounciness").Value),
                                    Bounding = Helpers.GetBoundingType(i.Element("Bounding").Value),
                                    ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                    Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                    Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                };

                foreach (var e in meshQuery)
                {
                    var entity = new Mesh(Core)
                    {
                        Name = e.Name,
                        FileName = e.FileName,
                        Visible = e.Visible,
                        Position = e.Position,
                        Rotation = e.Rotation,
                        Scale = e.Scale,
                        Mass = e.Mass,
                        StaticFriction = e.StaticFriction,
                        KineticFriction = e.KineticFriction,
                        Softness = e.Softness,
                        Bounciness = e.Bounciness,
                        Bounding = e.Bounding,
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<Mesh>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region Trigger
                var triggerQuery = from e in xDoc.Descendants("Triggers")
                                   from i in e.Descendants("Trigger")
                                   select new
                                   {
                                       Name = i.Element("Name").Value,
                                       Position = Helpers.GetVector3D(i.Element("Position").Value),
                                       Rotation = Helpers.GetVector3D(i.Element("Rotation").Value),
                                       Scale = Helpers.GetVector3D(i.Element("Scale").Value),
                                       Color = Helpers.GetVector3D(i.Element("Color").Value),
                                       ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                       Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                       Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                   };

                foreach (var e in triggerQuery)
                {
                    var entity = new Trigger(Core)
                    {
                        Name = e.Name,
                        Position = e.Position,
                        Rotation = e.Rotation,
                        Scale = e.Scale,
                        Color = Globals.RGBA(e.Color.x / 255f, e.Color.y / 255f, e.Color.z / 255f, 1),
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<Trigger>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region DirectionalLight
                var directionalLightQuery = from e in xDoc.Descendants("Lights")
                                            from i in e.Descendants("Directional")
                                            where Helpers.GetBool(i.Element("Visible").Value) == true
                                            select new
                                            {
                                                Name = i.Element("Name").Value,
                                                Direction = Helpers.GetVector3D(i.Element("Direction").Value),
                                                Color = Helpers.GetVector3D(i.Element("Color").Value),
                                                ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                                Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                                Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                            };

                foreach (var e in directionalLightQuery)
                {
                    var entity = new DirectionalLight(Core)
                    {
                        Name = e.Name,
                        Direction = e.Direction,
                        Color = System.Drawing.Color.FromArgb((int)e.Color.x, (int)e.Color.y, (int)e.Color.z),
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<DirectionalLight>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region PointLight
                var pointLightQuery = from e in xDoc.Descendants("Lights")
                                      from i in e.Descendants("Point")
                                      where Helpers.GetBool(i.Element("Visible").Value) == true
                                      select new
                                      {
                                          Name = i.Element("Name").Value,
                                          Position = Helpers.GetVector3D(i.Element("Position").Value),
                                          Radius = Helpers.GetFloat(i.Element("Radius").Value),
                                          Color = Helpers.GetVector3D(i.Element("Color").Value),
                                          ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                          Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                          Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                      };

                foreach (var e in pointLightQuery)
                {
                    var entity = new PointLight(Core)
                    {
                        Name = e.Name,
                        Position = e.Position,
                        Radius = e.Radius,
                        Color = System.Drawing.Color.FromArgb((int)e.Color.x, (int)e.Color.y, (int)e.Color.z),
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<PointLight>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region Particle
                var particleQuery = from e in xDoc.Descendants("Particles")
                                   from i in e.Descendants("Particle")
                                   select new
                                   {
                                       Name = i.Element("Name").Value,
                                       FileName = i.Element("FileName").Value,
                                       Position = Helpers.GetVector3D(i.Element("Position").Value),
                                       Rotation = Helpers.GetVector3D(i.Element("Rotation").Value),
                                       Scale = Helpers.GetVector3D(i.Element("Scale").Value),
                                       Visible = Helpers.GetBool(i.Element("Visible").Value),
                                       ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                       Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                       Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                   };

                foreach (var e in particleQuery)
                {
                    var entity = new Particle(Core)
                    {
                        Name = e.Name,
                        FileName = e.FileName,
                        Position = e.Position,
                        Rotation = e.Rotation,
                        Scale = e.Scale,
                        Visible = e.Visible,
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<Particle>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region Sound
                var soundQuery = from e in xDoc.Descendants("Sounds")
                                 from i in e.Descendants("Sound")
                                 select new
                                 {
                                     Name = i.Element("Name").Value,
                                     FileName = i.Element("FileName").Value,
                                     Position = Helpers.GetVector3D(i.Element("Position").Value),
                                     Volume = Helpers.GetFloat(i.Element("Volume").Value),
                                     Stopped = Helpers.GetBool(i.Element("Stopped").Value),
                                     Loop = Helpers.GetBool(i.Element("Loop").Value),
                                     Is3D = Helpers.GetBool(i.Element("Is3D").Value),
                                     ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                     Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                     Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                 };

                foreach (var e in soundQuery)
                {
                    var entity = new Sound(Core)
                    {
                        Name = e.Name,
                        FileName = e.FileName,
                        Position = e.Position,
                        Volume = e.Volume * Helpers.GameSettings.FXVolume / 100f,
                        Stopped = e.Stopped,
                        Loop = e.Loop,
                        Is3D = e.Is3D,
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<Sound>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region SkySphere
                var skySphereQuery = from e in xDoc.Descendants("Sky")
                                     from i in e.Descendants("SkySphere")
                                    select new
                                    {
                                        Rotation = Helpers.GetVector3D(i.Element("Rotation").Value),
                                        Scale = Helpers.GetVector3D(i.Element("Scale").Value),
                                        PolyCount = Helpers.GetInt(i.Element("PolyCount").Value), 
                                        FileName = i.Element("Texture").Value,
                                        ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                        Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                        Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                    };

                foreach (var e in skySphereQuery)
                {
                    var entity = new SkySphere(Core)
                    {
                        Rotation = e.Rotation,
                        Scale = e.Scale,
                        PolyCount = e.PolyCount,
                        FileName = e.FileName,
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<SkySphere>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region SkyBox
                var skyBoxQuery = from e in xDoc.Descendants("Sky")
                                  from i in e.Descendants("SkyBox")
                                     select new
                                     {
                                         FrontTexture = i.Element("FrontTexture").Value,
                                         BackTexture = i.Element("BackTexture").Value,
                                         LeftTexture = i.Element("LeftTexture").Value,
                                         RightTexture = i.Element("RightTexture").Value,
                                         TopTexture = i.Element("TopTexture").Value,
                                         BottomTexture = i.Element("BottomTexture").Value,
                                         ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                         Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                         Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                     };

                foreach (var e in skyBoxQuery)
                {
                    var entity = new SkyBox(Core)
                    {
                        FrontTexture = e.FrontTexture,
                        BackTexture = e.BackTexture,
                        LeftTexture = e.LeftTexture,
                        RightTexture = e.RightTexture,
                        TopTexture = e.TopTexture,
                        BottomTexture = e.BottomTexture,
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<SkyBox>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                #region Water
                var waterQuery = from e in xDoc.Descendants("WaterPlanes")
                                    from i in e.Descendants("Water")
                                     select new
                                     {
                                         Position = Helpers.GetVector3D(i.Element("Position").Value),
                                         Scale = Helpers.GetVector3D(i.Element("Scale").Value),
                                         ScriptEnabled = Helpers.GetBool(i.Descendants("CustomScript").Select(p => p.Element("Enabled")).FirstOrDefault().Value),
                                         Script = i.Descendants("CustomScript").Select(p => p.Element("Script")).FirstOrDefault().Value,
                                         Parameters = i.Descendants("CustomParameters").Descendants("Parameter").ToList()
                                     };

                foreach (var e in waterQuery)
                {
                    var entity = new Water(Core)
                    {
                        Position = e.Position,
                        Scale = e.Scale,
                        ScriptEnabled = e.ScriptEnabled,
                        Script = e.Script
                    };

                    SetCustomParameters(entity, e.Parameters);

                    Core.LoadComponent<Water>(entity);
                    gameObjects.Add(entity);
                }
                #endregion

                // Generate minimap for scene file (not global).
                if (scene.Value == true)
                {
                    var fi = new FileInfo(scene.Key);
                    var imageFile = Path.Combine(fi.DirectoryName, fi.Name.Replace(fi.Extension, ".png"));
                    //if (!File.Exists(imageFile))
                    {
                        var minimap = Helpers.CreateMinimap(gameObjects);
                        minimap.Save(imageFile, ImageFormat.Png);
                        minimap.Dispose();
                    }
                }

                // Get some information about the scene.
                skySphereExist = gameObjects.FindAll(o => o.GetType().Equals(typeof(SkySphere))).Count > 0;
                skyBoxExist = gameObjects.FindAll(o => o.GetType().Equals(typeof(SkyBox))).Count > 0;
                waterExists = Helpers.GetGameObjects<Water>(gameObjects).Count > 0;
            }
        }

        void CleanScene()
        {
            skySphereExist = false;
            skyBoxExist = false;
            waterExists = false;

            SoundManager.RemoveAllSounds();

            foreach (var entity in gameObjects)
            {
                Core.UnloadComponent((Component)entity);
            }

            gameObjects.Clear();

            //Atmosphere.SkyBox_SetTexture(-1, -1, -1, -1, -1, -1);
            Atmosphere.SkyBox_Enable(false);
            //Atmosphere.SkySphere_SetTexture(-1);
            Atmosphere.SkySphere_Enable(false);

            //LightEngine.DeleteAllLights();
            ////Scene.DestroyAllActors();
            ////Scene.DestroyAllLandscapes();
            //Scene.DestroyAllMeshes();
            //MaterialFactory.DeleteAllMaterials();

            ////Scene.DestroyAllMinimeshes();
            //Scene.DestroyAllParticleSystems();
        }

        public override void PreDraw()
        {
            if (waterExists)
            {
                // Reflect
                Helpers.GetGameObjects<Water>(gameObjects).ForEach(o => o.ReflectRS.StartRender());
                Helpers.GetGameObjects<Mesh>(gameObjects).ForEach(o => o.GetMesh().Render());
                if (skySphereExist) Atmosphere.SkySphere_Render();
                if (skyBoxExist) Atmosphere.SkyBox_Render();
                Helpers.GetGameObjects<Water>(gameObjects).ForEach(o => o.ReflectRS.EndRender());
                // Refract
                Helpers.GetGameObjects<Water>(gameObjects).ForEach(o => o.RefractRS.StartRender());
                Helpers.GetGameObjects<Mesh>(gameObjects).ForEach(o => o.GetMesh().Render());
                if (skySphereExist) Atmosphere.SkySphere_Render();
                if (skyBoxExist) Atmosphere.SkyBox_Render();
                Helpers.GetGameObjects<Water>(gameObjects).ForEach(o => o.RefractRS.EndRender());
            }
        }

        public override void Draw()
        {
            Scene.RenderAll(true, true);
        }

        public List<object> GetGameObjects()
        {
            return gameObjects;
        }

        public List<T> GetGameObjects<T>()
        {
            return Helpers.GetGameObjects<T>(gameObjects);
        }

        void SetCustomParameters(ObjectBase entity, List<XElement> parameters)
        {
            foreach (var param in parameters)
            {
                entity.Parameters.Add(new Parameter()
                {
                    Name = param.Element("Name").Value,
                    Value = param.Element("Value").Value
                });
            }
        }

        [ServiceDependency]
        public IScriptManagerService ScriptManager { private get; set; }
        [ServiceDependency]
        public ISoundManagerService SoundManager { private get; set; }
        [ServiceDependency]
        public IDebuggingBagService DebuggingBag { private get; set; }
    }

    public interface ISceneManagerService : IService
    {
        List<object> GetGameObjects();
        List<T> GetGameObjects<T>();
        void LoadLevel(string fileName);
        void UnloadLevel();
    }
}