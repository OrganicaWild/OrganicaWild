using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public static class PoissonDiskSampling
{
    //public static IEnumerable<Vector2> GeneratePoints(float width, float height, float radius,
    //    int numSamplesBeforeRejection = 30)
    //{
    //    float r2 = Mathf.Pow(radius, 2);
    //    float cellSize = radius * Mathf.Sqrt(0.5f);
    //    int gridWidth = Mathf.CeilToInt(width / cellSize);
    //    int gridHeight = Mathf.CeilToInt(width / cellSize);
    //    Vector2[] grid = new Vector2 [gridWidth * gridHeight];
    //    Queue<Vector2> queue = new Queue<Vector2>();

    //    yield return Sample(width/2, height/2);

    //    // Pick a random existing sample from the queue.
    //    pick: while (queue.Count > 0)
    //    {
    //        int i = (int) Random.value * queue.Count;
    //        Vector2 parent = queue.ToArray()[i];
    //        float seed = Random.value;

    //        // Make a new candidate.
    //        for (int j = 0; j < numSamplesBeforeRejection; ++j)
    //        {
    //            float a = 2 * Mathf.PI * (seed + j / (float) numSamplesBeforeRejection);
    //            float ra = radius + float.Epsilon;
    //            float x = parent[0] + ra * Mathf.Cos(a);
    //            float y = parent[1] + r * Mathf.Sin(a);

    //            // Accept candidates that are inside the allowed extent
    //            // and farther than 2 * radius to all existing samples.
    //            if (0 <= x && x < width && 0 <= y && y < height && Far(x, y))
    //            {
    //                yield return  Sample(x, y);
    //                goto pick;
    //            }
    //        }

    //        // If none of k candidates were accepted, remove it from the queue.
    //        Vector2 r = queue.Dequeue();
    //        if (i < queue.Count) queue.ToArray()[i] = r;
    //    }

    //    Vector2 Sample(float x, float y)
    //    {
    //        Vector2 sample = new Vector2(x, y);
    //        int position = (int) (gridWidth * (y / cellSize) + x / cellSize);
    //        grid[position] = sample;
    //        queue.Enqueue(sample);
    //        return sample;
    //    }

    //    bool Far(float x, float y)
    //    {
    //        int i = (int) (x / cellSize);
    //        int j = (int) (y / cellSize);
    //        int iMax = Mathf.Max(i - 2, 0);
    //        int jMax = Math.Max(j - 2, 0);
    //        int iMin = Math.Min(i + 3, gridWidth);
    //        int jMin = Math.Min(j + 3, gridHeight);
    //        for (int jTemp = jMax; jTemp < jMin; ++j)
    //        {
    //            int o = j * gridWidth;
    //            for (int iTemp = iMax; iTemp < iMin; ++i)
    //            {
    //                Vector2 s = grid[o + i];
    //                //if (s)
    //                //{
    //                    float dx = s[0] - x;
    //                    float dy = s[1] - y;
    //                    if (dx * dx + dy * dy < r2) return false;
    //                //}
    //            }
    //        }
    //        return true;
    //    }

    //}

    /// <summary>
    /// Generates Points on a plane. Around these points you can draw circles that do not intersect with a given radius.
    /// </summary>
    /// <param name="radius">desired radius</param>
    /// <param name="width">width of the plane</param>
    /// <param name="height">height of the plane</param>
    /// <param name="numSamplesBeforeRejection">the minimal number of times this function tries to find new points near each found point</param>
    /// <returns></returns>
    public static IEnumerable<Vector2> GeneratePoints(float radius, float width, float height, int numSamplesBeforeRejection = 30)
    {
        Vector2 sampleRegionSize = new Vector2(width, height);
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(sampleRegionSize / 2);
        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);
                if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }

        }

        return points;
    }

    private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}