using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Framework.Util
{
    public class Vector3Extensions
    {
        public static List<Vector3> GetConvexHull(List<Vector3> points)
        {
            List<Vector3> resultSet = new List<Vector3>();
            IEnumerable<Tuple<Vector3, Vector3>> pairs = points.SelectMany(x => points, Tuple.Create)
                .Where(tuple => tuple.Item1 != tuple.Item2);

            foreach ((Vector3 item1, Vector3 item2) in pairs)
            {
                bool valid = true;
                foreach (Vector3 point in points.Except(new List<Vector3> {item1, item2}))
                {
                    valid = IsPointOnRightSideOfLine(item1, item2, point) < 0;
                }

                if (valid)
                {
                    resultSet.Add(item1);
                    resultSet.Add(item2);
                }
            }

            return resultSet;
        }

        private static float IsPointOnRightSideOfLine(Vector3 a, Vector3 b, Vector3 x)
        {
            return (x.x - a.x) * (b.z - a.z) - (x.z - a.z) * (b.x - b.y);
        }
        
        public static Vector3 GetRandomNormalizedVector(Random random)
        {
            float x = (float) random.NextDouble() - 0.5f;
            float y = (float) random.NextDouble() - 0.5f;
            float z = (float) random.NextDouble() - 0.5f;

            return new Vector3(x, y, z).normalized;
        }
    }
}