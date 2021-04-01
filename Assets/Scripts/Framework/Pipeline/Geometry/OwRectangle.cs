using System.Collections.Generic;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwRectangle : OwPolygon
    {
        public OwRectangle(Vector2 start, float height, float width) : base(new List<Vector2>())
        {
            Vector2 b = start + new Vector2(0,height);
            Vector2 c = b + new Vector2(width, 0);
            Vector2 d = c - new Vector2(0, height);
            
            //first region is created in base constructor
            representation.Regions[0].Points.AddRange(new Point[] {start, b, c, d});
        }
    }
}