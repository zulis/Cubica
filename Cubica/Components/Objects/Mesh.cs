using System;
using System.IO;
using System.Linq;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Managers;
using System.Globalization;

namespace Cubica.Components.Objects
{
    partial class Mesh : ObjectBase, IMesh
    {
        // Physics variables.
        public CONST_TV_PHYSICSBODY_BOUNDING Bounding { get; set; }
        public float Bounciness { get; set; }
        public float KineticFriction { get; set; }
        public float Mass { get; set; }
        public float Softness { get; set; }
        public float StaticFriction { get; set; }
        public int PhysicsId { get; internal set; }

        // Private variables.
        TVMesh mesh;
        int materialIdx;
        private enum LightMode
        {
            None,
            Managed,
            Normal,
            Offset
        }

        public Mesh(ICore core) : base(core) { }

        public override void Initialize()
        {
            mesh = Scene.CreateMeshBuilder();

            switch (Helpers.GetFileFormat(FileName))
            {
                case Helpers.FileFormat.TVM:
                    mesh.LoadTVM(FileName, true, false);
                    break;
                case Helpers.FileFormat.X:
                    mesh.LoadXFile(FileName, true, false);
                    break;
            }

            if (Visible)
            {
                mesh.EnableFrustumCulling(true, true);
                mesh.ComputeNormals();
                mesh.ComputeBoundings();
                mesh.ComputeOctree();
                mesh.SetAlphaTest(true);
                mesh.SetCullMode(CONST_TV_CULLING.TV_BACK_CULL);
                mesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA);
                mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED);
                mesh.SetShadowCast(true, true);

                mesh.SetPosition(Position.x, Position.y, Position.z);
                mesh.SetRotation(Rotation.x, Rotation.y, Rotation.z);
                mesh.SetScale(Scale.x, Scale.y, Scale.z);

                // Physics
                SetPhysics();
            }
            else
            {
                // Hide mesh.
                for (var i = 0; i < mesh.GetGroupCount(); i++)
                    mesh.SetGroupEnable(i, false);
            }

            // Register object in Lua.
            ScriptManager.SetGlobal(Name, this);
        }

        public override void PostInitialize()
        {
            // Apply advanced texturing only if there are some lights.
            if (SceneManager.GetGameObjects().Count(o => o.GetType().Equals(typeof(DirectionalLight)) ||
                o.GetType().Equals(typeof(PointLight))) > 0)
            {
                LoadTextures();
            }

            if (ScriptEnabled)
            {
                ScriptManager.RegisterCustomFunctions(this);
                // Actually we do not need that. By the way it creates lag.
                //ScriptManager.RegisterFunctions(mesh);
                ScriptManager.DoString(Script);
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_INIT));
            }
        }

        public override void Dispose()
        {
            MaterialFactory.DeleteMaterial(materialIdx);
            Physics.DestroyBody(PhysicsId);
            mesh.Destroy();
            mesh = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            // Update position property since objects position can be changed from the script.
            Position = Physics.GetBodyPosition(PhysicsId);

            if (ScriptEnabled)
            {
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_UPDATE));
            }
        }

        public override void Draw()
        {
            if (ScriptEnabled)
            {
                ScriptManager.CallFunction(string.Format(CultureInfo.InvariantCulture, Constants.FUNCTION_STUB, Name, Constants.FUNCTION_DRAW));
            }
        }

        private void SetPhysics()
        {
            if (Mass == 0f)
            {
                if (!Bounding.Equals(CONST_TV_PHYSICSBODY_BOUNDING.TV_BODY_NONE))
                {
                    PhysicsId = Physics.CreateStaticMeshBody(mesh);
                }
            }
            else
            {
                PhysicsId = Physics.CreateMeshBody(Mass, mesh, Bounding, true);
            }

            SetPhysicsMaterial();
        }

        private void SetPhysicsMaterial()
        {
            if (PhysicsId != -1)
            {
                // There is a limit to 65 material groups,
                // so it hangs here
                //var physicsMatId = Physics.CreateMaterialGroup();
                //Physics.SetMaterialInteractionFriction(physicsMatId, -1, StaticFriction, KineticFriction);
                //Physics.SetMaterialInteractionSoftness(physicsMatId, -1, Softness);
                //Physics.SetMaterialInteractionBounciness(physicsMatId, -1, Bounciness);
                //Physics.SetBodyMaterialGroup(PhysicsId, physicsMatId);
                
                Physics.SetAutoFreeze(PhysicsId, false);
                Physics.SetBodyPosition(PhysicsId, Position.x, Position.y, Position.z);
                Physics.SetBodyRotation(PhysicsId, Rotation.x, Rotation.y, Rotation.z);
                Physics.SetDamping(PhysicsId, 0f, new TV_3DVECTOR(0f, 0f, 0f));
            }
        }

        private void LoadTextures()
        {
            //mesh.SetShadowCast(true, true);
            var lightMode = LightMode.Managed;

            for (var i = 0; i < mesh.GetGroupCount(); i++)
            {
                var textureInfo = TextureFactory.GetTextureInfo(mesh.GetTexture(i));

                // Skip if there is no texture.
                if (textureInfo.Filename.Equals(textureInfo.Name))
                    break;

                var normalTexture = string.Empty;
                var normalTextureName = string.Empty;
                var specularTexture = string.Empty;
                var specularTextureName = string.Empty;
                var heightTexture = string.Empty;
                var heightTextureName = string.Empty;

                var texture = textureInfo.Filename;

                var fileInfo = new FileInfo(textureInfo.Filename);
                normalTextureName = fileInfo.Name.Replace(fileInfo.Extension, Constants.TextureNormalSuffix + fileInfo.Extension);
                normalTexture = Directory.GetParent(textureInfo.Filename) + @"\" + normalTextureName;
                heightTextureName = fileInfo.Name.Replace(fileInfo.Extension, Constants.TextureHeightSuffix + fileInfo.Extension);
                heightTexture = Directory.GetParent(textureInfo.Filename) + @"\" + heightTextureName;
                specularTextureName = fileInfo.Name.Replace(fileInfo.Extension, Constants.TextureSpecularSuffix + fileInfo.Extension);
                specularTexture = Directory.GetParent(textureInfo.Filename) + @"\" + specularTextureName;
                //mesh.SetTextureEx((int)CONST_TV_LAYER.TV_LAYER_BASETEXTURE, Globals.GetTex(textureInfo.Name), i);

                if (File.Exists(normalTexture))
                {
                    var normalId = TextureFactory.LoadTexture(normalTexture, normalTextureName);
                    mesh.SetTextureEx((int)CONST_TV_LAYER.TV_LAYER_NORMALMAP, normalId, i);

                    if (lightMode != LightMode.Offset)
                    {
                        lightMode = LightMode.Normal;
                    }

                    if (File.Exists(specularTexture))
                    {
                        var alphaId = TextureFactory.LoadTexture(specularTexture);
                        var specularId = TextureFactory.AddAlphaChannel(normalId, alphaId, specularTexture);
                        mesh.SetTextureEx((int)CONST_TV_LAYER.TV_LAYER_SPECULARMAP, specularId, i);
                    }
                }

                if (File.Exists(heightTexture))
                {
                    var heightId = TextureFactory.LoadTexture(heightTexture);
                    mesh.SetTextureEx((int)CONST_TV_LAYER.TV_LAYER_HEIGHTMAP, heightId, i);
                    lightMode = LightMode.Offset;
                }
            }

            switch (lightMode)
            {
                case LightMode.Normal:
                    mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_BUMPMAPPING_TANGENTSPACE);
                    break;
                case LightMode.Offset:
                    mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_OFFSETBUMPMAPPING_TANGENTSPACE);
                    break;
                case LightMode.Managed:
                    mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED);
                    break;
                default:
                    mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_NONE);
                    break;
            }

            // Setup material
            TV_COLOR ambient = new TV_COLOR(0.25f, 0.25f, 0.25f, 1f);
            TV_COLOR diffuse = new TV_COLOR(1f, 1f, 1f, 1f);
            TV_COLOR specular = new TV_COLOR(0.35f, 0.35f, 0.35f, 1f);
            TV_COLOR emissive = new TV_COLOR(1f, 1f, 1f, 1f);
            float power = 100;

            materialIdx = MaterialFactory.CreateMaterial();
            MaterialFactory.SetAmbient(materialIdx, ambient.r, ambient.g, ambient.b, ambient.a);
            MaterialFactory.SetDiffuse(materialIdx, diffuse.r, diffuse.g, diffuse.b, diffuse.a);
            MaterialFactory.SetSpecular(materialIdx, specular.r, specular.g, specular.b, specular.a);
            MaterialFactory.SetEmissive(materialIdx, emissive.r, emissive.g, emissive.b, emissive.a);
            MaterialFactory.SetPower(materialIdx, power);
            mesh.SetMaterial(materialIdx);
        }

        TVMesh IMesh.GetMesh()
        {
            return mesh;
        }

        [ServiceDependency]
        public new ISceneManagerService SceneManager { private get; set; }
    }
}