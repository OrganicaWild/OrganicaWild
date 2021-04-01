using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Demo.Pipeline
{
    public class DebugDataStructure : MonoBehaviour
    {
        private OwPolygon poly;
        private OwPolygon poly2;
        private OwPolygon union;

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
            
            Debug.Log(poly.Contains(poly2));
        }

        private void OnDrawGizmos()
        {
            if (poly == null)
            {
                return;
            }

            poly.DrawDebug(Color.red);
            poly2.DrawDebug(Color.blue);
            union.DrawDebug(Color.green);
        }
    }
}