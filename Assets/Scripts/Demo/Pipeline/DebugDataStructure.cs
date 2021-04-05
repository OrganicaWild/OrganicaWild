using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
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

            union = poly.Xor(poly2);

            line0 = new OwLine(new Vector2(0, 0), new Vector2(10, 10));
            line1 = new OwLine(new Vector2(0, 10), new Vector2(10, 0));

            circle = new OwCircle(new Vector2(10,10), 5, 20);
            
            point = poly.Intersection(circle);
            Debug.Log(point);
            
            Debug.Log(poly.Contains(poly2));

            world = new GameWorld(new Area(poly));
            world.Root.AddChild(new Area(circle));
        }

        private void OnDrawGizmos()
        {
            if (poly == null)
            {
                return;
            }
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
            
            world.DrawDebug();
        }
    }
}