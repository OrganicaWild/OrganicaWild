using Tektosyne.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
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

        public void ScaleFromCentroid(Vector2 axis)
        {
            //do nothing
        }

        public override string ToString()
        {
            return $"{Position}";
        }

        public void DrawDebug(Color debugColor)
        {
            const float offset = 0.5f;
   
            Gizmos.color = debugColor;
            Vector3 coord = new Vector3(Position.x, 0, Position.y);
            Gizmos.DrawLine(coord + new Vector3(-offset, 0, -offset).normalized * offset,
                coord + new Vector3(offset, 0, offset).normalized * offset);
            Gizmos.DrawLine(coord + new Vector3(offset, 0, -offset).normalized * offset,
                coord + new Vector3(-offset, 0, offset).normalized * offset);

            Gizmos.DrawLine(coord + new Vector3(0, 0, -offset), coord + new Vector3(0, 0, offset));
            Gizmos.DrawLine(coord + new Vector3(offset, 0, 0), coord + new Vector3(-offset, 0, 0));
        }

        public static implicit operator OwPoint(PointD pointD)
        {
            return new OwPoint(new Vector2((float) pointD.X, (float) pointD.Y));
        }

        public static implicit operator PointD(OwPoint owPoint)
        {
            return new PointD(owPoint.Position.x, owPoint.Position.y);
        }
    }
}