using Cubica.Components.Objects;
using MTV3D65;

namespace Cubica.Managers
{
    partial class SceneManager
    {
        [RegisterFunction]
        public TV_2DVECTOR GetXBounds()
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;

            var meshList = Helpers.GetGameObjects<Mesh>(gameObjects);
            meshList.RemoveAll(o => o.GetCustParam("type").Equals("background"));

            foreach (var mesh in meshList)
            {
                if (mesh.Position.x < minX)
                    minX = mesh.Position.x;
                if (mesh.Position.x > maxX)
                    maxX = mesh.Position.x;
            }

            return new TV_2DVECTOR(minX, maxX);
        }

        [RegisterFunction]
        public TV_2DVECTOR GetYBounds()
        {
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            var meshList = Helpers.GetGameObjects<Mesh>(gameObjects);
            meshList.RemoveAll(o => o.GetCustParam("type").Equals("background"));

            foreach (var mesh in meshList)
            {
                if (mesh.Position.y < minY)
                    minY = mesh.Position.y;
                if (mesh.Position.y > maxY)
                    maxY = mesh.Position.y;
            }

            return new TV_2DVECTOR(minY, maxY);
        }

        [RegisterFunction]
        public TV_2DVECTOR GetZBounds()
        {
            var minZ = float.MaxValue;
            var maxZ = float.MinValue;

            var meshList = Helpers.GetGameObjects<Mesh>(gameObjects);
            meshList.RemoveAll(o => o.GetCustParam("type").Equals("background"));

            foreach (var mesh in meshList)
            {
                if (mesh.Position.z < minZ)
                    minZ = mesh.Position.z;
                if (mesh.Position.z > maxZ)
                    maxZ = mesh.Position.z;
            }

            return new TV_2DVECTOR(minZ, maxZ);
        }
    }
}
