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
            //if start or end is not inside, the line is for sure not inside
            if (!PolygonPointInteractor.Use().Contains(first, new OwPoint(second.Start)) ||
                !PolygonPointInteractor.Use().Contains(first, new OwPoint(second.End)))
            {
                return false;
            }

            //if concave polygon check that the line is not intersecting with any edges
            List<OwLine> polyLines = first.GetEdges();
            bool isIntersecting = false;
            foreach (OwLine edge in polyLines)
            {
                isIntersecting |= LineLineInteractor.Use().Intersect(edge, second).Any();
                if (isIntersecting)
                {
                    return false;
                }
            }

            return true;
        }
 
        public bool PartiallyContains(OwPolygon first, OwLine second)
        {
            //if either the start or end are inside. The line is for sure partially contained
            if (PolygonPointInteractor.Use().Contains(first, new OwPoint(second.Start)) ||
                PolygonPointInteractor.Use().Contains(first, new OwPoint(second.End)))
            {
                return true;
            }

            List<OwLine> polyLines = first.GetEdges();
            int intersections = 0;
            foreach (OwLine edge in polyLines)
            {
                bool isIntersecting =
                    LineLineInteractor.Use().Intersect(edge, second).Any();
                if (isIntersecting)
                {
                    intersections++;
                }
            }

            return intersections > 0 && intersections % 2 == 0;
        }

        public OwLine CalculateShortestPath(OwPolygon first, OwLine second)
        {
            OwLine shortest = new OwLine(new Vector2(float.MinValue, float.MinValue),
                new Vector2(float.MaxValue, float.MaxValue));

            foreach (OwLine polygonLine in first.GetEdges())
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
            OwPolygon extrudedSecond = new OwPolygon(new[] {second.Start, second.End, second.End + Vector2.one});
            OwPolygon intersectionPolygon = PolygonPolygonInteractor.Use().Intersection(first, extrudedSecond);

            if (intersectionPolygon.Representation.IsEmpty()) return Enumerable.Empty<IGeometry>();

            IEnumerable<OwLine> intersectingLines = CalculateIntersectingLines(second, intersectionPolygon);

            if (intersectingLines.Any()) return intersectingLines;
            return Enumerable.Empty<IGeometry>();
        }

        public float CalculateDistance(OwPolygon first, OwLine second)
        {
            return CalculateShortestPath(first, second).Length();
        }

        private IEnumerable<OwLine> CalculateIntersectingLines(OwLine originalLine, OwPolygon intersectionPolygon)
        {
            IEnumerable<OwLine> result = new List<OwLine>();
            LineLineInteractor interactor = LineLineInteractor.Use();

            foreach (Region representationRegion in intersectionPolygon.Representation.Regions)
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