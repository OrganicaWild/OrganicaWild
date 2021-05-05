using System;
using System.Collections.Generic;
using System.Linq;
using g3;
using Polybool.Net.Objects;
using Tektosyne.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwPolygon : IGeometry
    {
        public Polygon representation { get; private set; }

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

        public OwPolygon(Polygon polygon)
        {
            representation = new Polygon() {Regions = new List<Region>()};

            foreach (Region polygonRegion in polygon.Regions)
            {
                Region region = new Region() {Points = new List<Point>()};

                foreach (Point regionPoint in polygonRegion.Points)
                {
                    Point point = new Point(regionPoint.X, regionPoint.Y);
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

            representation.Regions.ForEach(region => region.Points = region.Points.Select(point =>
            {
                point -= centroid;
                point *= axis;
                point += centroid;
                return point;
            }).ToList());
        }

        public void MovePolygon(Vector2 moveTo)
        {
            Vector2 translate = moveTo - GetCentroid();
            
            representation.Regions.ForEach(region => region.Points = region.Points.Select(point =>
            {
                point += translate;
                return point;
            }).ToList());
        }

        public List<Vector2> GetPoints()
        {
            return representation.Regions.SelectMany(region => region.Points).Select(point => (Vector2) point).ToList();
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

     

        public OwPolygon GetConvexHull()
        {
            PointD[] hullPoints = GeoAlgorithms.ConvexHull(GetPoints().Select(x => new PointD(x.x, x.y)).ToArray());
            //convex hull algorithm revereses the resulting points, so they have to be revered before creating the new polygon
            OwPolygon hullPolygon = new OwPolygon(hullPoints.Select(x => new Vector2((float) x.X, (float) x.Y)).Reverse());
            return hullPolygon;
        }

        public List<GeneralPolygon2d> Getg3Polygon()
        {
            List<GeneralPolygon2d> result = new List<GeneralPolygon2d>();
            
            List<GeneralPolygon2d> polysToMesh = representation.Regions.Select(region =>
                new GeneralPolygon2d(new Polygon2d(region.Points.Select(x => new Vector2d((float) x.X, (float) x.Y))))).ToList();
            
            //sort descending area
            polysToMesh.Sort((f,s) => (int) (s.Area - f.Area));

            while (polysToMesh.Any())
            {
                GeneralPolygon2d polyToMesh = polysToMesh.First();

                foreach (GeneralPolygon2d possibleHole in polysToMesh.ToList())
                {
                    //Try add hole
                    try
                    {
                        //all holes must be reversed
                        possibleHole.Outer.Reverse();
                        polyToMesh.AddHole(possibleHole.Outer, true, true);
                        //if it is a hole remove it from the rest of polygons to be meshed
                        polysToMesh.Remove(possibleHole);
                    }
                    catch (Exception e)
                    {
                        //if it is is not a hole reverse back the polygon
                        Console.WriteLine(e);
                        possibleHole.Outer.Reverse();
                    }
                }

                //remove region after it is meshed
                polysToMesh.Remove(polyToMesh);
                result.Add(polyToMesh);
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