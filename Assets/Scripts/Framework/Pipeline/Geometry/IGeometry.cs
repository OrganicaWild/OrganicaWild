using UnityEngine;

namespace Framework.Pipeline
{
    public interface IGeometry
    {
        bool Contains(IGeometry other);

        bool PartiallyContains(IGeometry other);

        IGeometry GetShortestPathTo(IGeometry other);

        IGeometry Intersection(IGeometry other);

        float DistanceTo(IGeometry other);

        Vector2 GetCentroid();

        void DrawDebug(Color debugColor);
    }
}