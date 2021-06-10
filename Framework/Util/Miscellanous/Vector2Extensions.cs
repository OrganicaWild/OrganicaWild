using Tektosyne.Geometry;
using UnityEngine;
using Random = System.Random;


namespace Framework.Util
{
    public class Vector2Extensions
    {
        public static PointD Convert(Vector2 vector2)
        {
            return new PointD(vector2.x, vector2.y);
        }

        public static Vector2 Convert(PointD pointD)
        {
            return new Vector2((float) pointD.X, (float) pointD.Y);
        }

        public static Vector2 GetRandomNormalizedVector(Random random)
        {
            float x = (float) random.NextDouble() - 0.5f;
            float y = (float) random.NextDouble() - 0.5f;

            return new Vector2(x, y).normalized;
        }
    }
}