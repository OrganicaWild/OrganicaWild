using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    /// <summary>
    /// Geometry Interface for defining geometric shapes to represent IGameWorldObjects in the GameWorld data structure.
    /// </summary>
    public interface IGeometry
    {
        /// <summary>
        /// Get centroid of the defined geometric shape
        /// </summary>
        /// <returns></returns>
        Vector2 GetCentroid();

        /// <summary>
        /// Scale the geometric shape from the middle.
        /// </summary>
        /// <param name="scaleFactorPerAxis">scale factor on each axis</param>
        void ScaleFromCentroid(Vector2 scaleFactorPerAxis);

        /// <summary>
        /// Draw debug view of this geometric shape.
        /// Use this method inside of OnDrawGizmos().
        /// </summary>
        /// <param name="debugColor">color to draw the geometry in</param>
        /// <param name="offset">optional vector offset to add into geometry when drawing. This is to prevent potential occlusion.</param>
        void DrawDebug(Color debugColor, Vector3 offset = default);

        /// <summary>
        /// Create deep copy of geometric shape
        /// </summary>
        /// <returns>deep copy of geometric shape</returns>
        IGeometry Copy();
    }
}