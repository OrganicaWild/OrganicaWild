using System;
using System.Collections.Generic;
using System.Linq;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwCircle : OwPolygon
    {
        public OwCircle(Vector2 center, float radius, int resolution) : base(new List<Vector2>())
        {
            List<Vector2> pointList = new List<Vector2>();
            Vector2 up = new Vector2(1, 0);
            float theta = 2f * Mathf.PI / resolution;

            pointList.Add(center + up.normalized * radius);
            for (int i = 0; i < resolution; i++)
            {
                //rotate by desired angle for each side
                Vector2 rotated = new Vector2(
                        up.x * Mathf.Cos(theta) - up.y * Mathf.Sin(theta), 
                        up.x * Mathf.Sin(theta) + up.y * Mathf.Cos(theta));

                pointList.Add(center + rotated.normalized * radius);
                up = rotated;
            }

            representation.Regions[0].Points.AddRange(pointList.Select(vec2 => (Point) vec2));
        }
    }
}