using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Tektosyne.Geometry;
using UnityEngine;

namespace Demo.Pipeline
{
    public class DebugDataStructure : MonoBehaviour
    {
        private OwPolygon poly;
        private OwPolygon poly2;
        private OwPolygon union;
        private OwLine line0;
        private OwLine line1;
        private IGeometry point;
        private OwPolygon circle;
        private GameWorld world;
        private IGeometry line;
        private OwPoint point0;

        private void Start()
        {
            poly = new OwPolygon(new[]
            {
                new Vector2(0, 10),
                new Vector2(10, 10),
                new Vector2(10, 0)
            });
            
            poly2 = new OwPolygon(new[]
            {
                new Vector2(0, 0),
                new Vector2(10, 10),
                new Vector2(0, 10)
            });

            union = poly.Union(poly2);
            // var A = new PointD(1, 1);
            // var B = new PointD(10, 10);
            //
            //
            // var C = new PointD(1, 10);
            // var D = new PointD(10, 1);
            // var result = LineIntersection.Find(A, B, C, D);
            // Debug.Log(result);

            line0 = new OwLine(new Vector2(0, 0), new Vector2(10, 10));
            //point0 = new OwPoint(new Vector2(5, 5));
            
            line1 = new OwLine(new Vector2(0, 8), new Vector2(8, 0));
            line =  LineLineInteractor.use().Intersect(line0, line1);
/*
            circle = new OwCircle(new Vector2(10,10), 5, 20);
            
            point = poly.Intersection(circle);
            Debug.Log(point);
            
            Debug.Log(poly.Contains(poly2));

            world = new GameWorld(new Area(poly));
            world.Root.AddChild(new Area(circle));*/
        }

        private void OnDrawGizmos()
        {
            if (poly == null)
            {
                return;
            }
            
            // union.DrawDebug(Color.red, Vector2.zero);
            // //
            // poly.DrawDebug(Color.red);
            // // poly2.DrawDebug(Color.blue);
            // // union.DrawDebug(Color.green);
            //
            // // line0.DrawDebug(Color.green);
            // // line1.DrawDebug(Color.red);
            // //
            //
            // circle.DrawDebug(Color.magenta);
            // point.DrawDebug(Color.blue);
            
        
           line1.DrawDebug(Color.blue, Vector2.zero);
           line0.DrawDebug(Color.blue, Vector2.zero);
           line.DrawDebug(Color.red, Vector2.zero);
           //point0.DrawDebug(Color.yellow, Vector2.zero);
        }
    }
}