using System.Collections.Generic;
using System.Linq;
using Polybool.Net.Objects;
using Tektosyne.Geometry;

namespace Framework.Pipeline.Geometry.Interactors
{
    public class PolygonPointInteractor : IGeometryInteractor<OwPolygon, OwPoint>
    {
        private static PolygonPointInteractor instance;

        private PolygonPointInteractor()
        {
        }

        public static PolygonPointInteractor Use()
        {
            return instance ?? (instance = new PolygonPointInteractor());
        }

        public bool Contains(OwPolygon first, OwPoint second)
        {
            return Intersect(first, second).Count() != 0;
        }

        public bool PartiallyContains(OwPolygon first, OwPoint second)
        {
            return Contains(first, second);
        }

        public OwLine CalculateShortestPath(OwPolygon first, OwPoint second)
        {
            List<OwLine> paths = new List<OwLine>();

            foreach (OwLine owLine in first.GetLines())
            {
                paths.Add(LinePointInteractor.Use().CalculateShortestPath(owLine, second));
            }

            paths.Sort((line, owLine) => (int) (200 * (line.Length() - owLine.Length())));
            return paths.First();
        }

        public IEnumerable<IGeometry> Intersect(OwPolygon first, OwPoint second)
        {
            List<IGeometry> result = new List<IGeometry>();
            foreach (Region region in first.representation.Regions)
            {
                PolygonLocation singleResult = GeoAlgorithms.PointInPolygon(second,
                    region.Points.Select(point => (PointD) point).ToArray());

                if (singleResult == PolygonLocation.Inside)
                {
                    result.Add(new OwPoint(second.Position));
                }
            }

            return result;
        }

        public float CalculateDistance(OwPolygon first, OwPoint second)
        {
            return CalculateShortestPath(first, second).Length();
            
        }
    }
}