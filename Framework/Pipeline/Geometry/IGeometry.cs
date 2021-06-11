using UnityEngine;

namespace Framework.Pipeline
{
    public interface IGeometry
    {
        Vector2 GetCentroid();

        void ScaleFromCentroid(Vector2 axis);

        void DrawDebug(Color debugColor);
    }
}