using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwPoint : IGeometry
    {
        private readonly Vector2 position;

        public OwPoint(Vector2 position)
        {
            this.position = position;
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
            throw new System.NotImplementedException();
        }

        public float DistanceTo(IGeometry other)
        {
            Vector2 center = other.GetCentroid();
            return (center - GetCentroid()).magnitude;
        }

        public Vector2 GetCentroid()
        {
            return position;
        }

        public override string ToString()
        {
            return $"{position}";
        }

        public void DrawDebug(Color debugColor)
        {
            const float offset = 0.5f;
            
            Gizmos.color = debugColor;
            Vector3 coord = new Vector3(position.x, 0, position.y);
            Gizmos.DrawLine(coord + new Vector3(-offset, 0, -offset).normalized * offset, coord + new Vector3(offset, 0, offset).normalized * offset);
            Gizmos.DrawLine(coord + new Vector3(offset, 0, -offset).normalized * offset, coord + new Vector3(-offset, 0, offset).normalized * offset);
            
            Gizmos.DrawLine(coord + new Vector3(0, 0, -offset), coord + new Vector3(0, 0, offset));
            Gizmos.DrawLine(coord + new Vector3(offset, 0, 0), coord + new Vector3(-offset, 0, 0));
            
        }
    }
}