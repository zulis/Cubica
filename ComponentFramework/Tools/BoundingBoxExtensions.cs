using SlimDX;

namespace ComponentFramework.Tools
{
    public static class BoundingBoxExtensions
    {
        public static Vector3 Center(this BoundingBox box)
        {
            return (box.Minimum + box.Maximum) / 2;
        }

        public static Vector3 Size(this BoundingBox box)
        {
            return box.Maximum - box.Minimum;
        }
    }
}
