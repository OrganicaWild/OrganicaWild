using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using UnityEngine;
using UnityEngine.Assertions;

namespace Demo.Pipeline
{
    public class DebugDataStructure : MonoBehaviour
    {
        private IGeometry first;
        private IGeometry second;
        private IGeometry result;
        public Vector2 positionPoly0;
        private List<OwPoint> intersections = new List<OwPoint>();

        private void Start()
        {
            StartCoroutine(nameof(DoPolyLineTests));
        }

        private IEnumerator DoPolyLineTests()
        {
            //Polygon Line Tests
            first = new OwCircle(Vector2.zero, 5, 20);
            second = new OwLine(Vector2.zero, new Vector2(10, 10));

            //poly line full contains => false
            bool boolResult = PolygonLineInteractor.use().Contains((OwPolygon) first, (OwLine) second);
            Assert.IsFalse(boolResult);

            //poly line partial contains => true
            boolResult = PolygonLineInteractor.use().PartiallyContains((OwPolygon) first, (OwLine) second);
            Assert.IsTrue(boolResult);

            //poly line intersection ==> partial line
            result = PolygonLineInteractor.use().Intersect((OwPolygon) first, (OwLine) second);
            Assert.IsTrue(result is OwLine);
            yield return null;
            
            //poly line shortestpath => has path
            second = new OwLine(new Vector2(10, 0), new Vector2(0, 10));
            result = PolygonLineInteractor.use().CalculateShortestPath((OwPolygon) first, (OwLine) second);
            Assert.IsTrue(result is OwLine);
            yield return null;
        }

        private void OnDrawGizmos()
        {
            // if (first != null && second != null)
            // {
            //     first.DrawDebug(Color.green, Vector2.zero);
            //     second.DrawDebug(Color.blue, Vector2.zero);
            // }
            //
            // if (result != null)
            // {
            //     result.DrawDebug(Color.red, Vector2.zero);
            // }

            OwCircle circle0 = new OwCircle(positionPoly0, 2f, 20);
            OwCircle circle1 = new OwCircle(Vector2.one * 5f, 2f, 20);

            OwLine path = PolygonPolygonInteractor.use().CalculateShortestPath(circle0, circle1);
            
            circle0.DrawDebug(Color.yellow, Vector2.zero);
            circle1.DrawDebug(Color.blue, Vector2.zero);
            
            path.DrawDebug(Color.red, Vector2.zero);
            // foreach (OwPoint intersection in intersections)
            // {
            //     intersection.DrawDebug(Color.yellow, Vector2.zero);
            // }
            //
            // foreach (var line in (first as OwPolygon).GetLines())
            // {
            //     line.DrawDebug(Color.cyan, Vector2.zero);
            // }
        }
    }
}