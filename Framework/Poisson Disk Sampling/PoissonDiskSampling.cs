﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Framework.Poisson_Disk_Sampling
{
    public static class PoissonDiskSampling
    {
        /// <summary>
        /// Generates Points on a plane. Around these points you can draw circles that do not intersect with a given radius.
        /// </summary>
        /// <param name="radius">desired radius</param>
        /// <param name="width">width of the plane</param>
        /// <param name="height">height of the plane</param>
        /// <param name="numSamplesBeforeRejection">the minimal number of times this function tries to find new points near each found point</param>
        /// <param name="random">optional parameter to determine randomness</param>
        /// <returns></returns>
        public static IEnumerable<Vector2> GeneratePoints(float radius, float width, float height, int numSamplesBeforeRejection = 30, Random random = null)
        {
            random = random ?? new Random();

            Vector2 sampleRegionSize = new Vector2(width, height);
            float cellSize = radius / (float) Math.Sqrt(2);

            int[,] grid = new int[(int) Math.Ceiling(sampleRegionSize.x / cellSize), (int) Math.Ceiling(sampleRegionSize.y / cellSize)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2> {sampleRegionSize / 2};

            while (spawnPoints.Count > 0)
            {
                int spawnIndex = random.Next(0, spawnPoints.Count);
                Vector2 spawnCentre = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    double angle = random.NextDouble() * Math.PI * 2;
                    Vector2 dir = new Vector2((float) Math.Sin(angle), (float) Math.Cos(angle));

                    float scale = (float) random.NextDouble() * radius + radius;
                    Vector2 candidate = spawnCentre + dir * scale;
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
                int searchStartX = Math.Max(0, cellX - 2);
                int searchEndX = Math.Min(cellX + 2, grid.GetLength(0) - 1);
                int searchStartY = Math.Max(0, cellY - 2);
                int searchEndY = Math.Min(cellY + 2, grid.GetLength(1) - 1);

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
}