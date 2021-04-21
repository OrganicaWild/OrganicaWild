using System;
using System.Collections.Generic;
using Polybool.Net.Logic;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Geometry.Interactors
{
    public class PolygonPolygonInteractor : IGeometryInteractor<OwPolygon, OwPolygon>
    {
        
        private static PolygonPolygonInteractor instance;

        private PolygonPolygonInteractor()
        {
        }

        public static PolygonPolygonInteractor use()
        {
            return instance ?? (instance = new PolygonPolygonInteractor());
        }
        
        public bool Contains(OwPolygon first, OwPolygon second)
        {
            Polygon result = SegmentSelector.Union(first.representation, second.representation);
            // if the result equals this polygon if the other polygon is not present in the union result and this polygon is not changed
            return result.Equals(first.representation);
        }

        public bool PartiallyContains(OwPolygon first, OwPolygon second)
        {
            Polygon intersection = SegmentSelector.Union(first.representation, second.representation);
            //if the intersection of the two polygons is not empty the second polygon is partially contained in the first
            return !intersection.IsEmpty();
        }

        public OwLine CalculateShortestPath(OwPolygon first, OwPolygon second)
        {
            OwLine shortest = new OwLine(Vector2.zero, new Vector2(float.MaxValue, float.MaxValue));
            
            foreach (OwLine linesFirst in first.GetLines())
            {
               OwLine potentiallyShortest = PolygonLineInteractor.use().CalculateShortestPath(second, linesFirst);
               if (shortest.Length() > potentiallyShortest.Length())
               {
                   shortest = potentiallyShortest;
               }
            }
            
            return shortest;
        }

        public IGeometry Intersect(OwPolygon first, OwPolygon second)
        {
            return Difference(first, second);
        }

        public float CalculateDistance(OwPolygon first, OwPolygon second)
        {
            return CalculateShortestPath(first, second).Length();
        }

        public OwPolygon Union(OwPolygon first, OwPolygon second)
        {
            PolySegments segments0 = Polybool.Net.Logic.PolyBool.Segments(first.representation);
            PolySegments segments1 = Polybool.Net.Logic.PolyBool.Segments(second.representation);

            CombinedPolySegments combined = Polybool.Net.Logic.PolyBool.Combine(segments0, segments1);

            List<Segment> result = SegmentSelector.Union(combined.Combined);
            List<Region> x =  Polybool.Net.Logic.PolyBool.SegmentChainer(result);

            return new OwPolygon(new Polygon(x));

            // Polygon union = SegmentSelector.Union(first.representation, second.representation);
            //
            // return new OwPolygon(union);
        }

        public OwPolygon Intersection(OwPolygon first, OwPolygon second)
        {
            Polygon intersection = SegmentSelector.Intersect(first.representation, second.representation);
            return new OwPolygon(intersection);
        }

        public OwPolygon Xor(OwPolygon first, OwPolygon second)
        {
            Polygon xor = SegmentSelector.Xor(first.representation, second.representation);
            return new OwPolygon(xor);
        }

        public OwPolygon Difference(OwPolygon first, OwPolygon second)
        {
            Polygon difference = SegmentSelector.Difference(first.representation, second.representation);
            return new OwPolygon(difference);
        }

        public OwPolygon DifferenceReversed(OwPolygon first, OwPolygon second)
        {
            Polygon difference = SegmentSelector.DifferenceRev(first.representation, second.representation);
            return new OwPolygon(difference);
        }
    }
}