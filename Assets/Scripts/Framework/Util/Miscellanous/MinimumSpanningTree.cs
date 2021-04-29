using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Util
{
    public static class MinimumSpanningTree
    {
        public static List<OwLine> ByDistance(List<OwPoint> points)
        {
            List<OwLine> pq = new List<OwLine>();

            foreach (OwPoint point0 in points)
            {
                foreach (OwPoint point1 in points)
                {
                    if (point0 != point1)
                    {
                        pq.Add(new OwLine(point0.Position, point1.Position));
                    }
                }
            }

            //create dictionary for the union find data structure
            Dictionary<Vector2, int> rosetta = new Dictionary<Vector2, int>();
            int i = 0;
            foreach (OwPoint point in points)
            {
                if (!rosetta.ContainsKey(point.Position))
                {
                    rosetta.Add(point.Position, i);
                    i++;
                }
            }


            //sort by length
            pq.Sort((line0, line1) => (int) (line0.Length() - line1.Length()));
            List<OwLine> mst = new List<OwLine>();

            //simple kruskal implementation after Robert Sedgewick et al. from Algorithms Fourth Edition
            UnionFind uf = new UnionFind(points.Count);

            while (pq.Any() && mst.Count < points.Count - 1)
            {
                OwLine edge = pq.First();
                pq.Remove(edge);

                int v = rosetta[edge.Start];
                int w = rosetta[edge.End];

                if (uf.Connected(v, w))
                {
                    continue;
                }

                uf.Union(v, w);
                mst.Add(edge);
            }

            return mst;
        }
    }
}