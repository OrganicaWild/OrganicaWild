using System;
using System.Collections.Generic;
using System.Linq;
using g3;
using Polybool.Net.Objects;
using Tektosyne.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    /// <summary>
    /// Implements an arbitrary polygon as an IGeometry
    /// </summary>
    public class OwPolygon : IGeometry
    {
        internal Polygon Representation { get; private set; }

        public OwPolygon(IEnumerable<Vector2> points)
        {
            Representation =
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
            Representation = InnerCopy(polygon);
        }

        public OwPolygon(OwPolygon owPolygon)
        {
            Representation = InnerCopy(owPolygon.Representation);
        }

        private Polygon InnerCopy(Polygon polygon)
        {
            Polygon representation = new Polygon() {Regions = new List<Region>()};

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

            return representation;
        }
        
        /// <summary>
        /// Get the Centroid of this polygon.
        /// </summary>
        /// <returns>point in the centroid of this polygon</returns>
        public Vector2 GetCentroid()
        {
            Vector2 sum = Vector2.zero;
            int n = 0;
            foreach (Region representationRegion in Representation.Regions)
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

        /// <summary>
        /// Scale this polygon from its centroid.
        /// This method may not behave as expected when used with concave polygons.
        /// </summary>
        /// <param name="scaleFactorPerAxis">scale factor on each axis</param>
        public void ScaleFromCentroid(Vector2 scaleFactorPerAxis)
        {
            Vector2 centroid = GetCentroid();

            Representation.Regions.ForEach(region => region.Points = region.Points.Select(point =>
            {
                point -= centroid;
                point *= scaleFactorPerAxis;
                point += centroid;
                return point;
            }).ToList());
        }

        /// <summary>
        /// Move this polygon to a specified position, so that the centroid of this polygon is on the specified position.
        /// </summary>
        /// <param name="moveTo">position to move to</param>
        public void MovePolygon(Vector2 moveTo)
        {
            Vector2 translate = moveTo - GetCentroid();
            
            Representation.Regions.ForEach(region => region.Points = region.Points.Select(point =>
            {
                point += translate;
                return point;
            }).ToList());
        }

        /// <summary>
        /// Get all points of this polygon.
        /// The order of the points can be arbitrary.
        /// </summary>
        /// <returns>list with all points of polygon</returns>
        public List<Vector2> GetPoints()
        {
            return Representation.Regions.SelectMany(region => region.Points).Select(point => (Vector2) point).ToList();
        }

        /// <summary>
        /// Get all edges of polygon as lines.
        /// </summary>
        /// <returns>list of edge as lines</returns>
        public List<OwLine> GetEdges()
        {
            List<OwLine> lines = new List<OwLine>();

            foreach (Region representationRegion in Representation.Regions)
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

        /// <summary>
        /// Get axis aligned bounding box of this polygon
        /// </summary>
        /// <returns>bounding box</returns>
        public Rect GetBoundingBox()
        {
            //TODO: change return type to OwRectangle
            float minX = Single.MaxValue;
            float maxX = Single.MinValue;
            float minY = Single.MaxValue;
            float maxY = Single.MinValue;
            
            foreach (Vector2 point in GetPoints())
            {
                minX = Mathf.Min(minX, point.x);
                minY = Mathf.Min(minY, point.y); 
                maxX = Mathf.Max(maxX, point.x);
                maxY = Mathf.Max(maxY, point.y);
            }

            return new Rect(new Vector2(minX, minY), new Vector2(maxX - minX, maxY - minY));
        }
        
        
        /// <summary>
        /// Get convex hull of this polygon
        /// </summary>
        /// <returns>convex hull of polygon</returns>
        public OwPolygon GetConvexHull()
        {
            PointD[] hullPoints = GeoAlgorithms.ConvexHull(GetPoints().Select(x => new PointD(x.x, x.y)).ToArray());
            //convex hull algorithm reverses the resulting points, so they have to be reversed before creating the new polygon
            OwPolygon hullPolygon = new OwPolygon(hullPoints.Select(x => new Vector2((float) x.X, (float) x.Y)).Reverse());
            return hullPolygon;
        }
        
        internal List<GeneralPolygon2d> Getg3Polygon()
        {
            List<GeneralPolygon2d> result = new List<GeneralPolygon2d>();
            
            List<GeneralPolygon2d> polysToMesh = Representation.Regions.Select(region =>
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
                    catch (Exception)
                    {
                        //if it is is not a hole reverse back the polygon
                        //Console.WriteLine(e);
                        possibleHole.Outer.Reverse();
                    }
                }

                //remove region after it is meshed
                polysToMesh.Remove(polyToMesh);
                result.Add(polyToMesh);
            }

            return result;
        }

        public double GetArea()
        {
            double sum = 0;
            List<GeneralPolygon2d> polygons = Getg3Polygon();
            foreach (GeneralPolygon2d generalPolygon2d in polygons)
            {
                double size = generalPolygon2d.Area;
                sum += size;
            }

            return sum;
        }

        public void DrawDebug(Color debugColor, Vector3 offset = default)
        {
            Gizmos.color = debugColor;
            foreach (Region representationRegion in Representation.Regions)
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
                        Gizmos.DrawLine(new Vector3(prevVec2.x, 0, prevVec2.y) + offset,
                            new Vector3(currentVec2.x, 0, currentVec2.y) + offset);
                        prev = representationRegionPoint;
                    }
                }

                Vector2 lastVec2 = prev;
                Vector2 firstVec2 = first;
                Gizmos.DrawLine(new Vector3(lastVec2.x, 0, lastVec2.y) + offset, new Vector3(firstVec2.x, 0, firstVec2.y) + offset);
            }
        }

        public IGeometry Copy()
        {
            return new OwPolygon(Representation);
        }
    }
}