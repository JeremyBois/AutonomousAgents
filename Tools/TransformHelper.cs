using Microsoft.Xna.Framework;


namespace Tools
{
    using Tools.Vector;

    /// <summary>
    /// Provides helpers for transformations
    /// See : http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2D/Coll_Detection_Matrices.php
    /// </summary>
    public static class TransformHelper
    {
        /// <summary>
        /// Convert a point coordinates from local to world space using matrix operations.
        /// Matrix order is as follow:
        ///     ---> Local Scaling -> Local Rotation -> Local Translation
        /// Multiplication order is then (in matrix multiplication `*` means after):
        ///     ---> M = T * R * S
        /// But because rotation are done based on origin, we need to first translate
        /// to parent position (in world coordinates (Tw))
        /// So the final matrix multiplication are has below
        ///     ---> M = Tl * Rl * Sl * Tw
        /// Matrix order is then as follow:
        ///     ---> Translate to Origin -> Local Scaling -> Local Rotation -> Local Translation
        /// </summary>
        /// <param name="localPos">The Vector coordinates in local space.</param>
        /// <param name="heading">The x axis of the local space.</param>
        /// <param name="perp">The y axis of the local space.</param>
        /// <param name="parentPos">The position of the parent in world space.</param>
        /// <param name="scale">The scale in local space.</param>
        public static Vector2 LocalToWorldByMatrix(Vector2 localPos, Vector2 heading, Vector2 perp, Vector2 parentPos, Vector2 scale)
        {
            Matrix localMatrix = CreateRotationMatrix(heading, perp) *
                                 Matrix.CreateScale(scale.V3()) *
                                 Matrix.CreateTranslation(parentPos.V3());

            return Vector2.Transform(localPos, localMatrix);
        }

        /// <summary>
        /// Convert a point coordinates from local to world space using matrix operations.
        /// Matrix order is as follow:
        ///     ---> Local Scaling -> Local Rotation -> Local Translation
        /// Multiplication order is then (in matrix multiplication `*` means after):
        ///     ---> M = T * R * S
        /// But because rotation are done based on origin, we need to first translate
        /// to parent position (in world coordinates (Tw))
        /// So the final matrix multiplication are has below
        ///     ---> M = Tl * Rl * Sl * Tw
        /// Matrix order is then as follow:
        ///     ---> Translate to Origin -> Local Scaling -> Local Rotation -> Local Translation
        /// </summary>
        /// <param name="localPos">The Vector coordinates in local space.</param>
        /// <param name="heading">The x axis of the local space.</param>
        /// <param name="perp">The y axis of the local space.</param>
        /// <param name="parentPos">The position of the parent in world space.</param>
        public static Vector2 LocalToWorldByMatrix(Vector2 localPos, Vector2 heading, Vector2 perp, Vector2 parentPos)
        {
            Matrix localMatrix = CreateRotationMatrix(heading, perp) *
                                 Matrix.CreateTranslation(parentPos.V3());

            return Vector2.Transform(localPos, localMatrix);
        }

        /// <summary>
        /// Convert a point coordinates from local to world space using matrix operations.
        /// Matrix order is as follow:
        ///     ---> Local Scaling -> Local Rotation -> Local Translation
        /// Multiplication order is then (in matrix multiplication `*` means after):
        ///     ---> M = T * R * S
        /// But because rotation are done based on origin, we need to first translate
        /// to parent position (in world coordinates (Tw))
        /// So the final matrix multiplication are has below
        ///     ---> M = Tl * Rl * Sl * Tw
        /// Matrix order is then as follow:
        ///     ---> Translate to Origin -> Local Scaling -> Local Rotation -> Local Translation
        /// Because we are going from World to Local we need the Matrix inverse.
        /// </summary>
        /// <param name="worldPos">The Vector coordinates in world space.</param>
        /// <param name="heading">The x axis of the local space.</param>
        /// <param name="perp">The y axis of the local space.</param>
        /// <param name="parentPos">The position of the parent in world space.</param>
        /// <param name="scale">The scale in local space.</param>
        public static Vector2 WorldToLocalByMatrix(Vector2 worldPos, Vector2 heading, Vector2 perp, Vector2 parentPos, Vector2 scale)
        {
            Matrix worldMatrix = Matrix.Invert
                (
                    CreateRotationMatrix(heading, perp) *
                    Matrix.CreateScale(scale.V3()) *
                    Matrix.CreateTranslation(parentPos.V3())
                );

            return Vector2.Transform(worldPos, worldMatrix);
        }

        /// <summary>
        /// Convert a point coordinates from local to world space using matrix operations.
        /// Matrix order is as follow:
        ///     ---> Local Scaling -> Local Rotation -> Local Translation
        /// Multiplication order is then (in matrix multiplication `*` means after):
        ///     ---> M = T * R * S
        /// But because rotation are done based on origin, we need to first translate
        /// to parent position (in world coordinates (Tw))
        /// So the final matrix multiplication are has below
        ///     ---> M = Tl * Rl * Sl * Tw
        /// Matrix order is then as follow:
        ///     ---> Translate to Origin -> Local Scaling -> Local Rotation -> Local Translation
        /// Because we are going from World to Local we need the Matrix inverse.
        /// </summary>
        /// <param name="worldPos">The Vector coordinates in world space.</param>
        /// <param name="heading">The x axis of the local space.</param>
        /// <param name="perp">The y axis of the local space.</param>
        /// <param name="parentPos">The position of the parent in world space.</param>
        public static Vector2 WorldToLocalByMatrix(Vector2 worldPos, Vector2 heading, Vector2 perp, Vector2 parentPos)
        {
            Matrix worldMatrix = Matrix.Invert
                (
                    CreateRotationMatrix(heading, perp) *
                    Matrix.CreateTranslation(parentPos.V3())
                );

            return Vector2.Transform(worldPos, worldMatrix);
        }

        /// <summary>
        /// Convert a point coordinates from local to world space.
        /// </summary>
        /// <param name="localPos">The Vector coordinates in local space.</param>
        /// <param name="heading">The x axis of the local space.</param>
        /// <param name="perp">The y axis of the local space.</param>
        /// <param name="parentPos">The position of the parent in world space (.</param>
        public static Vector2 LocalToWorld(Vector2 localPos, Vector2 heading, Vector2 perp, Vector2 parentPos)
        {
            return new Vector2
            (
                localPos.X * heading.X + localPos.Y * perp.X + parentPos.X,
                localPos.X * heading.Y + localPos.Y * perp.Y + parentPos.Y
            );
        }

        /// <summary>
        /// Get local matrix to get from local to world coordinates.
        /// </summary>
        /// <param name="heading">The x axis of the local space.</param>
        /// <param name="perp">The y axis of the local space.</param>
        public static Matrix CreateRotationMatrix(Vector2 heading, Vector2 perp)
        {
            Matrix localMatrix = Matrix.Identity;
            localMatrix.M11 = heading.X;
            localMatrix.M12 = perp.X;
            localMatrix.M21 = heading.Y;
            localMatrix.M22 = perp.Y;

            return localMatrix;
        }
    }
}
