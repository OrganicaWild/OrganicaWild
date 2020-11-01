using System;
using UnityEngine;

namespace Util
{
    public static class RandomUtil
    {
        private static double spare;
        private static bool hasSpare = false;
        private static System.Random random =new System.Random();
        
        public static double GenerateGaussian(double mean, double stdDev) {
            if (hasSpare) {
                hasSpare = false;
                return spare * stdDev + mean;
            } else {
                double u, v, s;
                do {
                    u = random.NextDouble() * 2 - 1;
                    v = random.NextDouble() * 2 - 1;
                    s = u * u + v * v;
                } while (s >= 1 || s == 0);
                s = Math.Sqrt(-2.0 * Math.Log(s) / s);
                spare = v * s;
                hasSpare = true;
                return mean + stdDev * u * s;
            }
        }
    }
}