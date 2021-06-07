using System;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwLine : IGeometry
    {
        public Vector2 Start { get; private set; }
        public Vector2 End { get; private set; }

        public OwLine(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        
        public Vector2 GetCentroid()
        {
            //simply returns the middle point of the line
            return End - Start / 2f;
        }

        public void ScaleFromCentroid(Vector2 axis)
        {
            Vector2 centroid = GetCentroid();
            Start -= centroid;
            End -= centroid;

            Start *= axis;
            End *= axis;

            Start += centroid;
            End += centroid;
        }

        public void DrawDebug(Color debugColor)
        {
            Gizmos.color = debugColor;
         
            Gizmos.DrawLine(new Vector3(Start.x, 0, Start.y), new Vector3(End.x, 0, End.y));
        }

        public float Length()
        {
            return (End - Start).magnitude;
        }

        protected bool Equals(OwLine other)
        {
            float eps = 0.0001f;

            bool startX = Math.Abs(Start.x - other.Start.x) < eps;
            bool startY = Math.Abs(Start.y - other.Start.y) < eps;
            bool endX = Math.Abs(End.x - other.End.x) < eps;
            bool endY = Math.Abs(End.y - other.End.y) < eps;
            return startX && startY && endX && endY;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((OwLine) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ End.GetHashCode();
            }
        }
    }
}