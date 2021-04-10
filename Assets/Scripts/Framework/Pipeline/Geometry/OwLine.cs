using System;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwLine : IGeometry
    {
        public Vector2 Start { get; }
        public Vector2 End { get; }

        public OwLine(Vector2 start, Vector2 end)
        {
            this.Start = start;
            this.End = end;
        }

        public bool Contains(IGeometry other)
        {
            return false;
        }

        public bool PartiallyContains(IGeometry other)
        {
            return false;
        }

        public IGeometry GetShortestPathTo(IGeometry other)
        {
            throw new System.NotImplementedException();
        }

        public IGeometry Intersection(IGeometry other)
        {
            if (other is OwLine otherLine)
            {
                return new OwPoint(Intersection(otherLine));
            }

            return new OwInvalidGeometry();
        }

        public float DistanceTo(IGeometry other)
        {
            Vector2 center = other.GetCentroid();
            return (center - GetCentroid()).magnitude;
        }

        public Vector2 GetCentroid()
        {
            //simply returns the middle point of the line
            return End - Start / 2f;
        }

        public void DrawDebug(Color debugColor, Vector2 coordinateSystemCenter)
        {
            Gizmos.color = debugColor;
            Vector3 center = new Vector3(coordinateSystemCenter.x, 0, coordinateSystemCenter.y);
            Gizmos.DrawLine(center + new Vector3(Start.x, 0, Start.y), center + new Vector3(End.x, 0, End.y));
        }


        private Vector2 Intersection(OwLine other)
        {
            //TODO: move eps to some sort of config or global value
            const double eps = 0.000001d;

            Vector2 CD = other.End - other.Start;
            Vector2 AB = End - Start;
            Vector2 AC = other.Start - Start;

            double det = 1d / (AB.y * CD.x - AB.x * CD.y);

            double alpha = det * (-CD.y * AC.x + CD.x * AC.y);
            double beta = det * (-AB.y * AC.x + AB.x * AC.y);

            if (0 < alpha && alpha < 1 && alpha > eps && 1 - alpha > eps && beta > eps && 1 - beta > eps && 0 < beta &&
                beta < 1)
            {
                return Start + AB * (float) alpha;
            }

            return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        private static float Det(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        public float Length()
        {
            return (End - Start).magnitude;
        }
    }
}