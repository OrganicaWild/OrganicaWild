using System.Collections.Generic;
using System.Linq;

namespace Framework.Pipeline.Geometry
{
    public class PolygonLineInteractor : IGeometryInteractor<OwPolygon, OwLine>
    {
        public bool Contains(OwPolygon first, OwLine second)
        {
            throw new System.NotImplementedException();
        }

        public bool PartiallyContains(OwPolygon first, OwLine second)
        {
            throw new System.NotImplementedException();
        }

        public OwLine CalculateShortestPath(OwPolygon first, OwLine second)
        {
            List<OwLine> all = new List<OwLine>();

            foreach (OwLine polygonLine in first.GetLines())
            {
                all.Add( LineLineInteractor.use().CalculateShortestPath(polygonLine, second));
            }
            all.Sort((line, owLine) => (int) (200 * (line.Length() - owLine.Length())));

            return all.First();
        }

        public IGeometry Intersect(OwPolygon first, OwLine second)
        {

            throw new System.NotImplementedException();
        }

        public float CalculateDistance(OwPolygon first, OwLine second)
        {
            return CalculateShortestPath(first, second).Length();
        }
    }
}