using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Pipeline.Geometry.Interactors
{
    public class LinePointInteractor : IGeometryInteractor<OwLine, OwPoint>
    {
        private static LinePointInteractor instance;

        private LinePointInteractor()
        {
        }

        public static LinePointInteractor Use()
        {
            return instance ?? (instance = new LinePointInteractor());
        }

        public bool Contains(OwLine first, OwPoint second)
        {
            IGeometry intersect = Intersect(first, second).First();
            return intersect is OwPoint;
        }

        [Obsolete("Use Contains() instead")]
        public bool PartiallyContains(OwLine first, OwPoint second)
        {
            // a point cannot be partially contained on a line 
            return Contains(first, second);
        }

        public OwLine CalculateShortestPath(OwLine first, OwPoint second)
        {
            //this formula only works in 2D, never in 3D, that's different
            Vector2 P3 = second.Position;
            Vector2 P1 = first.Start;
            Vector2 P2 = first.End;

            float u = ((P3.x - P1.x) * (P2.x - P1.x) + (P3.y - P1.y) * (P2.y - P1.y)) /
                      ((P2 - P1).magnitude * (P2 - P1).magnitude);

            //clamp result
            if (u < 0)
            {
                u = 0;
            }

            if (u > 1)
            {
                u = 1;
            }

            Vector2 result = P1 + u * (P2 - P1);

            return new OwLine(result, P3);
        }

        public IEnumerable<IGeometry> Intersect(OwLine first, OwPoint second)
        {
            //https://stackoverflow.com/questions/17692922/check-is-a-point-x-y-is-between-two-points-drawn-on-a-straight-line
            Vector2 A = first.Start;
            Vector2 B = first.End;
            Vector2 AB = first.Start - first.End;
            Vector2 C = second.Position;

            float d0 = (A - C).magnitude + (B - C).magnitude;
            float d1 = AB.magnitude;

            if (Math.Abs(d0 - d1) < 0.001)
            {
                return new []{ new OwPoint(C) };
            }

            return new[] { new OwInvalidGeometry() };
        }

        public float CalculateDistance(OwLine first, OwPoint second)
        {
            return CalculateShortestPath(first, second).Length();
        }
    }
}