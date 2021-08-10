using System;
using Tektosyne.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    /// <summary>
    /// Implements an single point as an IGeometry
    /// </summary>
    public class OwPoint : IGeometry
    {
        public Vector2 Position { get; }

        public OwPoint(Vector2 position)
        {
            this.Position = position;
        }

        public Vector2 GetCentroid()
        {
            return Position;
        }

        public void ScaleFromCentroid(Vector2 scaleFactorPerAxis)
        {
            //do nothing
        }

        public override string ToString()
        {
            return $"{Position}";
        }

        public void DrawDebug(Color debugColor, Vector3 offset = default)
        {
            const float sizeOfStar = 1f;
   
            Gizmos.color = debugColor;
            Vector3 coord = new Vector3(Position.x, 0, Position.y) + offset;
            Gizmos.DrawLine(coord + new Vector3(-sizeOfStar, 0, -sizeOfStar).normalized * sizeOfStar,
                coord + new Vector3(sizeOfStar, 0, sizeOfStar).normalized * sizeOfStar);
            Gizmos.DrawLine(coord + new Vector3(sizeOfStar, 0, -sizeOfStar).normalized * sizeOfStar,
                coord + new Vector3(-sizeOfStar, 0, sizeOfStar).normalized * sizeOfStar);

            Gizmos.DrawLine(coord + new Vector3(0, 0, -sizeOfStar), coord + new Vector3(0, 0, sizeOfStar));
            Gizmos.DrawLine(coord + new Vector3(sizeOfStar, 0, 0), coord + new Vector3(-sizeOfStar, 0, 0));
        }

        public IGeometry Copy()
        {
            return new OwPoint(Position);
        }

        public static implicit operator OwPoint(PointD pointD)
        {
            return new OwPoint(new Vector2((float) pointD.X, (float) pointD.Y));
        }

        public static implicit operator PointD(OwPoint owPoint)
        {
            return new PointD(owPoint.Position.x, owPoint.Position.y);
        }

        protected bool Equals(OwPoint other)
        {
            float eps = 0.0001f;
            bool startX = Math.Abs(Position.x - other.Position.x) < eps;
            bool startY = Math.Abs(Position.y - other.Position.y) < eps;
            return startX && startY;
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

            return Equals((OwPoint) obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}