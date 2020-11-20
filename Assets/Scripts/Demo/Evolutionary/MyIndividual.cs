using System;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using Util;
using Random = UnityEngine.Random;

namespace Demo
{
    public class MyIndividual : AbstractNsga2Individual
    {
        private readonly int height;
        private readonly int width;
        private readonly int mutationPercentage;

        /**
         * 0 = floor
         * 1 = player
         * 2 = enemy
         */
        public int[,] map;

        public int[] mappedPositions;

        /**
         * 1 player position x and y
         * 2 enemy positions x and y
         */
        private const int SizeOfData = 6;

        private readonly double[] data;

        public MyIndividual(int height, int width, int mutationPercentage,
            AbstractNsga2FitnessFunction<MyIndividual>[] fitnessFunctions) : base(fitnessFunctions)
        {
            this.height = height;
            this.width = width;
            this.mutationPercentage = mutationPercentage;

            data = new double[SizeOfData];
            for (int i = 0; i < SizeOfData; i++)
            {
                data[i] = Random.value;
            }
            
            BuildMap();
        }

        private MyIndividual(int height, int width, int mutationPercentage, double[] genes,
            IFitnessFunction[] fitnessFunctions) : base(fitnessFunctions)
        {
            data = genes;
            this.height = height;
            this.width = width;
            this.mutationPercentage = mutationPercentage;
            BuildMap();
        }

        private void BuildMap()
        {
            map = new int[height, width];
            mappedPositions = new int[SizeOfData];

            int playerX = (int) (data[0] * width);
            int playerY = (int) (data[1] * height);
            map[playerY, playerX] = 1;
            mappedPositions[0] = playerX;
            mappedPositions[1] = playerY;

            int enemyIndex = 2;
            for (int i = 0; i < 2; i++)
            {
                int enemyX = (int) (data[enemyIndex] * width);
                mappedPositions[enemyIndex] = enemyX;
                enemyIndex++;

                int enemyY = (int) (data[enemyIndex] * height);
                mappedPositions[enemyIndex] = enemyY;
                map[enemyY, enemyX] = 2;
            }
        }

        public override INsga2Individual MakeOffspring(INsga2Individual parent2)
        {
            if (!(parent2 is MyIndividual other))
                throw new ArgumentException("second parent must be of same type as first parent!");

            double[] newGenes = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                double thisGene = data[i];
                double otherGene = other.data[i];

                double newGene = Random.Range(0, 1) < 0.5 ? thisGene : otherGene;
                newGenes[i] = newGene;
            }

            MyIndividual child = new MyIndividual(height, width, mutationPercentage, newGenes, FitnessFunctions);
            child.Mutate();

            return child;
        }

        private void Mutate()
        {
            for (int i = 0; i < SizeOfData; i++)
            {
                if (Random.Range(0, 100) < mutationPercentage)
                {
                    double random = RandomUtil.GenerateGaussian(0, 0.5);
                    data[i] += random;
                    if (data[i] < 0)
                    {
                        data[i] = 0;
                    }

                    if (data[i] > 1)
                    {
                        data[i] = 0.99;
                    }
                }
            }
        }
    }
}