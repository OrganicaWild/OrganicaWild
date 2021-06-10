using System.Collections.Generic;
using Framework.Pipeline;
using Framework.Pipeline.Geometry;
using g3;

namespace Framework.Util.Miscellanous
{
    public class PolygonWithHoleInteractions
    {
        public static bool Contains(GeneralPolygon2d polygon2d, OwPoint point)
        {
            if (polygon2d.Contains(new Vector2d(point.Position.x, point.Position.y)))
            {
                return true;
            }

            return false;
        }
    }
}