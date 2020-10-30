using System;
using Evolutionary.Framework;

namespace Evolutionary
{
    public class RandomVectorGenoPhenoCombination : IGenoPhenoCombination
    {
        protected double[] data;
        private readonly int size;

        public RandomVectorGenoPhenoCombination( int size)
        {
            data = new double[size];
            this.size = size;
            InitializeRandomData();
        }

        private void InitializeRandomData()
        {
            var rand = new Random();

            for (int i = 0; i < size; i++)
            {
                data[i] = rand.NextDouble();
            }
        }
    }
}