using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwInvalidGeometry : IGeometry
    {
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
            return new OwInvalidGeometry();
        }

        public IGeometry Intersection(IGeometry other)
        {
            return new OwInvalidGeometry();
        }

        public float DistanceTo(IGeometry other)
        {
            return float.PositiveInfinity;
        }

        public Vector2 GetCentroid()
        {
            return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        public void DrawDebug(Color debugColor, Vector2 coordinateSystemCenter)
        {
            Debug.LogError("Tried drawing Invalid Geometry.");
        }
    }
}