using System.Collections.Generic;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwRectangle : OwPolygon
    {
        public OwRectangle(Vector2 start, Vector2 end) : base(new List<Vector2>())
        {
            Vector2 c = start + new Vector2(0,end.y);
            Vector2 b = start + new Vector2(end.x, 0);
            Vector2 d = start + end;
            
            //first region is created in base constructor
            representation.Regions[0].Points.AddRange(new Point[] {start, b, d, c});
        }
    }
}