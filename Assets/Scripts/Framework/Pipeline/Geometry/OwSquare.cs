using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwSquare : OwRectangle
    {
        public OwSquare(Vector2 start, float sideLength) : base(start, sideLength, sideLength)
        {
        }
    }
}