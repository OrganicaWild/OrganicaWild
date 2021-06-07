using System;

namespace Framework.Util
{
    public static class RandomUtil
    {
        private static double Spare;
        private static bool HasSpare;
        private static readonly Random Random = new Random();

        public static double GenerateGaussian(double mean, double stdDev)
        {
            if (HasSpare)
            {
                HasSpare = false;
                return Spare * stdDev + mean;
            }
            else
            {
                double u, v, s;
                do
                {
                    u = Random.NextDouble() * 2 - 1;
                    v = Random.NextDouble() * 2 - 1;
                    s = u * u + v * v;
                } while (s >= 1 || s == 0);

                s = Math.Sqrt(-2.0 * Math.Log(s) / s);
                Spare = v * s;
                HasSpare = true;
                return mean + stdDev * u * s;
            }
        }
    }
}