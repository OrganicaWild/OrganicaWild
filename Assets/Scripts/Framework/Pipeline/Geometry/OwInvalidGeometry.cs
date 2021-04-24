using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwInvalidGeometry : IGeometry
    {
        public Vector2 GetCentroid()
        {
            return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        public void ScaleFromCentroid(Vector2 axis)
        {
           
        }

        public void DrawDebug(Color debugColor)
        {
            Debug.LogError("Tried drawing Invalid Geometry.");
        }
    }
}