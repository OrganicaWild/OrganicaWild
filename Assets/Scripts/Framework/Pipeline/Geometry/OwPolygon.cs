using System;
using System.Collections.Generic;
using System.Linq;
using Polybool.Net.Logic;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwPolygon : IGeometry
    {
        protected readonly Polygon representation;

        public OwPolygon(IEnumerable<Vector2> points)
        {
            representation =
                new Polygon(new List<Region>(new[]
                {
                    new Region()
                    {
                        Points = points.Select(vector2 => (Point) vector2).ToList()
                    }
                }));
        }

        private OwPolygon(Polygon polygon)
        {
            representation = new Polygon() {Regions = new List<Region>()};

            foreach (Region polygonRegion in polygon.Regions)
            {
                Region region = new Region() {Points = new List<Point>()};

                foreach (Point regionPoint in polygonRegion.Points)
                {
                    region.Points.Add(new Point(regionPoint.X, regionPoint.Y));
                }

                representation.Regions.Add(region);
            }
        }

        public bool Contains(IGeometry other)
        {
            //is polygon other fully contained in this?
            if (other is OwPolygon otherPoly)
            {
                Polygon result = SegmentSelector.Union(representation, otherPoly.representation);
                // if the result equals this polygon if the other polygon is not present in the union result and this polygon is not changed
                return result.Equals(representation);
            }

            //default false for any undefined IGeometry Implementations
            return false;
        }

        public bool PartiallyContains(IGeometry other)
        {
            if (other is OwPolygon otherPoly)
            {
                Polygon intersection = SegmentSelector.Union(representation, otherPoly.representation);
                //if the intersection of the two polygons is not empty the second polygon is partially contained in the first
                return !intersection.IsEmpty();
            }

            return false;
        }

        public IGeometry GetShortestPathTo(IGeometry other)
        {
            throw new System.NotImplementedException();
        }

        public IGeometry Intersection(IGeometry other)
        {
            if (other is OwPolygon otherPoly)
            {
                return Difference(otherPoly);
            }

            if (other is OwLine line)
            {
            }

            return new OwInvalidGeometry();
        }

        public float DistanceTo(IGeometry other)
        {
            Vector2 center = other.GetCentroid();
            return (center - GetCentroid()).magnitude;
        }

        public Vector2 GetCentroid()
        {
            //TODO: This center is not weighted since our vertices do not have weights.
            //according to this : https://stackoverflow.com/questions/2832771/find-the-centroid-of-a-polygon-with-weighted-vertices
            //if we leave it like this its just called the Centroid : https://en.wikipedia.org/wiki/Centroid

            Vector2 sum = Vector2.zero;
            int n = 0;
            foreach (Region representationRegion in representation.Regions)
            {
                foreach (Point representationRegionPoint in representationRegion.Points)
                {
                    n++;
                    sum += representationRegionPoint;
                }
            }

            if (n == 0)
            {
                return Vector2.zero;
            }

            return sum / n;
        }

        public OwPolygon Union(OwPolygon other)
        {
            Polygon union = SegmentSelector.Union(representation, other.representation);

            return new OwPolygon(union);
        }

        public OwPolygon Intersection(OwPolygon other)
        {
            Polygon intersection = SegmentSelector.Intersect(representation, other.representation);
            return new OwPolygon(intersection);
        }

        public OwPolygon Xor(OwPolygon other)
        {
            Polygon xor = SegmentSelector.Xor(representation, other.representation);
            return new OwPolygon(xor);
        }

        public OwPolygon Difference(OwPolygon other)
        {
            Polygon difference = SegmentSelector.Difference(representation, other.representation);
            return new OwPolygon(difference);
        }

        public OwPolygon DifferenceReversed(OwPolygon other)
        {
            Polygon difference = SegmentSelector.DifferenceRev(representation, other.representation);
            return new OwPolygon(difference);
        }

        public void DrawDebug(Color debugColor, Vector2 coordinateSystemCenter)
        {
            Vector3 center = new Vector3(coordinateSystemCenter.x, 0, coordinateSystemCenter.y);
            Gizmos.color = debugColor;
            foreach (Region representationRegion in representation.Regions)
            {
                Point first = null;
                Point prev = null;
                foreach (Point representationRegionPoint in representationRegion.Points)
                {
                    if (first == null)
                    {
                        first = representationRegionPoint;
                        prev = representationRegionPoint;
                    }
                    else
                    {
                        Vector2 prevVec2 = prev;
                        Vector2 currentVec2 = representationRegionPoint;
                        Gizmos.DrawLine(center + new Vector3(prevVec2.x, 0, prevVec2.y),
                            center + new Vector3(currentVec2.x, 0, currentVec2.y));
                        prev = representationRegionPoint;
                    }
                }

                Vector2 lastVec2 = prev;
                Vector2 firstVec2 = first;
                Gizmos.DrawLine(new Vector3(lastVec2.x, 0, lastVec2.y), new Vector3(firstVec2.x, 0, firstVec2.y));
            }
        }
    }
}