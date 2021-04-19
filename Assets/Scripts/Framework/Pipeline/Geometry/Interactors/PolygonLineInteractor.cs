using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Pipeline.Geometry.Interactors
{
    public class PolygonLineInteractor : IGeometryInteractor<OwPolygon, OwLine>
    {
        
        private static PolygonLineInteractor instance;

        private PolygonLineInteractor()
        {
        }

        public static PolygonLineInteractor use()
        {
            return instance ?? (instance = new PolygonLineInteractor());
        }
        
        public bool Contains(OwPolygon first, OwLine second)
        {
            bool startContained = PolygonPointInteractor.use().Contains(first, new OwPoint(second.Start));
            bool endContained = PolygonPointInteractor.use().Contains(first, new OwPoint(second.End));

            return startContained && endContained;
        }

        public bool PartiallyContains(OwPolygon first, OwLine second)
        {
            bool startContained = PolygonPointInteractor.use().Contains(first, new OwPoint(second.Start));
            bool endContained = PolygonPointInteractor.use().Contains(first, new OwPoint(second.End));

            return startContained || endContained;
        }

        public OwLine CalculateShortestPath(OwPolygon first, OwLine second)
        {
            OwLine shortest = new OwLine(Vector2.zero, new Vector2(float.MaxValue, float.MaxValue));

            foreach (OwLine polygonLine in first.GetLines())
            {
                OwLine potentiallyShortest = LineLineInteractor.use().CalculateShortestPath(polygonLine, second);
                if (shortest.Length() > potentiallyShortest.Length())
                {
                    shortest = potentiallyShortest;
                }
            }

            return shortest;
        }

        public IGeometry Intersect(OwPolygon first, OwLine second)
        {
            bool startContained = PolygonPointInteractor.use().Contains(first, new OwPoint(second.Start));
            bool endContained = PolygonPointInteractor.use().Contains(first, new OwPoint(second.End));

            if (startContained && endContained)
            {
                return new OwLine(second.Start, second.End);
            }

            if (!startContained && !endContained)
            {
                return new OwInvalidGeometry();
            }

            //line is partially contained --> result is new line from inner point to intersection
            
            List<OwPoint> intersections = new List<OwPoint>();
            foreach (OwLine edge in first.GetLines())
            {
                IGeometry edgeIntersection = LineLineInteractor.use().Intersect(edge, second);

                if (edgeIntersection is OwPoint point)
                {
                    intersections.Add(point);
                }
            }

            if (intersections.Count >= 1)
            {
                return startContained ? new OwLine(second.Start, intersections[0].Position) : new OwLine(second.End, intersections[0].Position);
            }

            return new OwInvalidGeometry();

        } 

        public float CalculateDistance(OwPolygon first, OwLine second)
        {
            return CalculateShortestPath(first, second).Length();
        }
    }
}