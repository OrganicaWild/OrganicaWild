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

        public void DrawDebug(Color debugColor, Vector3 offset = default)
        {
            Debug.LogError("Tried drawing Invalid Geometry.");
        }

        public IGeometry Copy()
        {
            return new OwInvalidGeometry();
        }
    }
}