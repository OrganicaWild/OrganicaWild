using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    /// <summary>
    /// Implements an axis aligned Square as an IGeometry
    /// </summary>
    public class OwSquare : OwRectangle
    {
        public OwSquare(Vector2 start, float sideLength) : base(start, start + new Vector2(sideLength, sideLength))
        {
        }
    }
}