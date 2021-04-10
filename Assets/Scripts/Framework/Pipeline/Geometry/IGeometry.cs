using UnityEngine;

namespace Framework.Pipeline
{
    public interface IGeometry
    {
        Vector2 GetCentroid();

        void DrawDebug(Color debugColor, Vector2 coordinateSystemCenter);
    }
}