using Microsoft.Xna.Framework;

namespace Tools.Vector
{
    public static class VectorExtensions
    {
        public static Vector2 TruncateExt (this Vector2 vector, float maxLength)
        {
            if ( vector.Length() > maxLength )
            {
                vector.Normalize();
                vector *= maxLength;
            }
            return vector;
        }

        /// <summary>
        /// Get the perpendicular vector (clockwise).
        /// ----> heading
        /// |
        /// |
        /// |
        /// perp
        /// </summary>
        public static Vector2 PerpExt(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        /// <summary>
        /// Return -1 if angle between both is clockwise else 1.
        /// Rotation are positive then done clockwise (https://msdn.microsoft.com/en-us/library/s0s56wcf(v=vs.110).aspx)
        /// </summary>
        public static float SignExt(this Vector2 vector, Vector2 other)
        {
            // Clockwise if negative determinant (2D cross product)
            // Clockwise must be positive because of rotation positive in clockwise direction
            return (vector.X * other.Y < other.X * vector.Y) ? 1 : -1;
        }

        /// <summary>
        /// Create a Vector3 from a Vector2
        /// </summary>
        public static Vector3 V3 (this Vector2 v)
        {
            return new Vector3 (v.X, v.Y, 0.0f);
        }
    }
}
