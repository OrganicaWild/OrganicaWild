using System.Collections.Generic;
using System.Linq;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Geometry.Interactors
{
    public class PolygonLineInteractor : IGeometryInteractor<OwPolygon, OwLine>
    {
        
        private static PolygonLineInteractor instance;

        private PolygonLineInteractor()
        {
        }

        public static PolygonLineInteractor Use()
        {
            return instance ?? (instance = new PolygonLineInteractor());
        }
        
        public bool Contains(OwPolygon first, OwLine second)
        {
            IEnumerable<IGeometry> intersectionLines = Intersect(first, second);

            if (intersectionLines.Count() != 1) return false;

            OwLine intersectionLine = intersectionLines.First() as OwLine;
            return intersectionLine != null
                   && (intersectionLine.Start == second.Start && intersectionLine.End == second.End
                       || intersectionLine.Start == second.End && intersectionLine.End == second.Start);
        }

        public bool PartiallyContains(OwPolygon first, OwLine second)
        {
            IEnumerable<IGeometry> intersectionLines = Intersect(first, second);

            return intersectionLines.Count() != 0;
        }

        public OwLine CalculateShortestPath(OwPolygon first, OwLine second)
        {
            OwLine shortest = new OwLine(new Vector2(float.MinValue, float.MinValue), new Vector2(float.MaxValue, float.MaxValue));

            foreach (OwLine polygonLine in first.GetLines())
            {
                OwLine potentiallyShortest = LineLineInteractor.Use().CalculateShortestPath(polygonLine, second);
                if (shortest.Length() > potentiallyShortest.Length())
                {
                    shortest = potentiallyShortest;
                }
            }

            return shortest;
        }

        public IEnumerable<IGeometry> Intersect(OwPolygon first, OwLine second)
        {
            OwPolygon extrudedSecond = new OwPolygon(new [] { second.Start, second.End, second.End + Vector2.one });
            OwPolygon intersectionPolygon = PolygonPolygonInteractor.Use().Intersection(first, extrudedSecond);
            
            if (intersectionPolygon.representation.IsEmpty()) return new []{ new OwInvalidGeometry() };

            IEnumerable<OwLine> intersectingLines = CalculateIntersectingLines(second, intersectionPolygon);

            if (intersectingLines.Any()) return intersectingLines; 
            return new[] { new OwInvalidGeometry() };
        }

        public float CalculateDistance(OwPolygon first, OwLine second)
        {
            return CalculateShortestPath(first, second).Length();
        }

        private IEnumerable<OwLine> CalculateIntersectingLines(OwLine originalLine, OwPolygon intersectionPolygon)
        {
            IEnumerable<OwLine> result = new List<OwLine>();
            LineLineInteractor interactor = LineLineInteractor.Use();

            foreach (Region representationRegion in intersectionPolygon.representation.Regions)
            {
                Point[] points = representationRegion.Points.ToArray();
                Vector2 previousPoint = points.First();
                OwLine currentLine;

                for (int i = 1; i < points.Length; i++)
                {
                    currentLine = new OwLine(previousPoint, points[i]);
                    if (interactor.Contains(originalLine, currentLine)) result.Append(currentLine);
                }

                currentLine = new OwLine(previousPoint, points.First());
                if (interactor.Contains(originalLine, currentLine)) result.Append(currentLine);
            }

            return result;
        }
    }
}