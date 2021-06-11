using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwSquare : OwRectangle
    {
        public OwSquare(Vector2 start, float sideLength) : base(start, start + new Vector2(sideLength, sideLength))
        {
        }
    }
}