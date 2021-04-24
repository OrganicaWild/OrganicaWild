using System;
using System.Collections.Generic;
using System.Linq;
using Habrador_Computational_Geometry;
using Polybool.Net.Logic;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwPolygon : IGeometry
    {
        public Polygon representation { get; private set; }
        private List<Vector2> points;

        public OwPolygon(IEnumerable<Vector2> points)
        {
            this.points = points.ToList();
            representation =
                new Polygon(new List<Region>(new[]
                {
                    new Region()
                    {
                        Points = points.Select(vector2 => (Point) vector2).ToList()
                    }
                }));
        }

        public OwPolygon(Polygon polygon)
        {
            points = new List<Vector2>();
            representation = new Polygon() {Regions = new List<Region>()};

            foreach (Region polygonRegion in polygon.Regions)
            {
                Region region = new Region() {Points = new List<Point>()};

                foreach (Point regionPoint in polygonRegion.Points)
                {
                    Point point = new Point(regionPoint.X, regionPoint.Y);
                    points.Add(point);
                    region.Points.Add(point);
                }

                representation.Regions.Add(region);
            }
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

        public void ScaleFromCentroid(Vector2 axis)
        {
            Vector2 centroid = GetCentroid();

            for (int index = 0; index < points.Count; index++)
            {
                Vector2 point = points[index];
                point -= centroid;
                point *= axis;
                point += centroid;

                points[index] = point;
            }

            representation =
                new Polygon(new List<Region>(new[]
                {
                    new Region()
                    {
                        Points = points.Select(vector2 => (Point) vector2).ToList()
                    }
                }));
        }

        public List<Vector2> GetPoints()
        {
            return points;
        }

        public List<OwLine> GetLines()
        {
            List<OwLine> lines = new List<OwLine>();

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
                        lines.Add(new OwLine(prev, representationRegionPoint));
                        prev = representationRegionPoint;
                    }
                }

                lines.Add(new OwLine(prev, first));
            }

            return lines;
        }

        public List<Vector3> GetTriangulation()
        {
            representation.Regions.Sort(((region, region1) => region1.Points.Count - region.Points.Count));

            List<Vector2> pp = representation.Regions.First().Points.Select(x => (Vector2) x).ToList();
            List<MyVector2> myVector2s = pp.Select(point => new MyVector2(point.x, point.y)).ToList();

            HashSet<Triangle2> triangles = _EarClipping.Triangulate(myVector2s);

            List<Vector3> result = new List<Vector3>();

            foreach (Triangle2 triangle in triangles)
            {
                var p1 = new Vector3(triangle.p1.x, triangle.p1.y);
                var p2 = new Vector3(triangle.p2.x, triangle.p2.y);
                var p3 = new Vector3(triangle.p3.x, triangle.p3.y);

                result.Add(p1);
                result.Add(p2);
                result.Add(p3);
            }

            return result;
        }

        public void DrawDebug(Color debugColor)
        {
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
                        Gizmos.DrawLine(new Vector3(prevVec2.x, 0, prevVec2.y),
                            new Vector3(currentVec2.x, 0, currentVec2.y));
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