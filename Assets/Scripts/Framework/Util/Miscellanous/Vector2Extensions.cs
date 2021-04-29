using Tektosyne.Geometry;
using UnityEngine;

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
    }
}