using System;
using UnityEngine;

namespace Framework.Util.Miscellanous
{
    public class FloydWarshall
    {
        public const int INF = 99999;

        private static void Print(int[,] distance, int verticesCount)
        {
            string printStr = "Shortest distances between every pair of vertices:";

            for (int i = 0; i < verticesCount; ++i)
            {
                for (int j = 0; j < verticesCount; ++j)
                {
                    if (distance[i, j] == INF)
                        printStr += "INF".PadLeft(7);
                    else
                        printStr += distance[i, j].ToString().PadLeft(7);
                }

                printStr += "\n";
            }

            Debug.Log(printStr);
        }

        public static int[,] Execute(int[,] adjMatrix, int verticesCount)
        {
            int[,] distance = new int[verticesCount, verticesCount];

            for (int i = 0; i < verticesCount; ++i)
            for (int j = 0; j < verticesCount; ++j)
                distance[i, j] = adjMatrix[i, j];

            for (int k = 0; k < verticesCount; ++k)
            {
                for (int i = 0; i < verticesCount; ++i)
                {
                    for (int j = 0; j < verticesCount; ++j)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                            distance[i, j] = distance[i, k] + distance[k, j];
                    }
                }
            }

#if UNITY_EDITOR
            Print(distance, verticesCount);
#endif

            return distance;
        }
    }
}