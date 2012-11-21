using MTV3D65;
using SlimDX;

namespace ComponentFramework.Tools
{
    public static class VectorExtensions
    {
        #region TV3D-SlimDX Conversion

        public static Matrix ToMatrix(this TV_3DMATRIX matrix)
        {
            return new Matrix
            {
                M11 = matrix.m11, M12 = matrix.m12, M13 = matrix.m13, M14 = matrix.m14,
                M21 = matrix.m21, M22 = matrix.m22, M23 = matrix.m23, M24 = matrix.m24,
                M31 = matrix.m31, M32 = matrix.m32, M33 = matrix.m33, M34 = matrix.m34,
                M41 = matrix.m41, M42 = matrix.m42, M43 = matrix.m43, M44 = matrix.m44
            };
        }
        public static Vector4 ToVector4(this TV_4DVECTOR vector)
        {
            return new Vector4(vector.x, vector.y, vector.z, vector.w);
        }
        public static Vector3 ToVector3(this TV_3DVECTOR vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }
        public static Vector2 ToVector2(this TV_2DVECTOR vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static TV_3DMATRIX ToTvMatrix(this Matrix matrix)
        {
            return new TV_3DMATRIX
            {
                m11 = matrix.M11, m12 = matrix.M12, m13 = matrix.M13, m14 = matrix.M14,
                m21 = matrix.M21, m22 = matrix.M22, m23 = matrix.M23, m24 = matrix.M24,
                m31 = matrix.M31, m32 = matrix.M32, m33 = matrix.M33, m34 = matrix.M34,
                m41 = matrix.M41, m42 = matrix.M42, m43 = matrix.M43, m44 = matrix.M44
            };
        }
        public static TV_4DVECTOR ToTvVector(this Vector4 vector)
        {
            return new TV_4DVECTOR(vector.X, vector.Y, vector.Z, vector.W);
        }
        public static TV_3DVECTOR ToTvVector(this Vector3 vector)
        {
            return new TV_3DVECTOR(vector.X, vector.Y, vector.Z);
        }
        public static TV_2DVECTOR ToTvVector(this Vector2 vector)
        {
            return new TV_2DVECTOR(vector.X, vector.Y);
        }

        #endregion

        #region Swizzling

        public static Vector3 XYZ(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Vector2 XZ(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Z);
        }
        public static Vector2 XY(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        #endregion

        #region Matrix Extensions 

        public static Vector3 GetRightVector(this Matrix matrix)
        {
            return matrix.get_Rows(0).XYZ();
        }

        public static Vector3 GetUpVector(this Matrix matrix)
        {
            return matrix.get_Rows(1).XYZ();
        }

        public static Vector3 GetForwardVector(this Matrix matrix)
        {
            return matrix.get_Rows(2).XYZ();
        }

        public static Vector3 GetTranslation(this Matrix matrix)
        {
            return matrix.get_Rows(3).XYZ();
        }

        #endregion
    }
}
