using System;
using Evolutionary.Framework;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Util;
using Random = UnityEngine.Random;

namespace Evolutionary.Standard
{
    public class StandardGenoPhenoCombination : IGenoPhenoCombination
    {
        /**
         * 1 player position x and y
         * 2 enemy positions x and y
         */
        private readonly int size = 6;

        private double[] data;

        private int height;
        
        private int width;

        private int mutationPercentage;

        /**
         * 0 = floor
         * 1 = player
         * 2 = enemy
         */
        private int[,] map;

        private int[] mappedPositions;

        public StandardGenoPhenoCombination(int height, int width, int mutationPercentage)
        {
            this.height = height;
            this.width = width;
            this.mutationPercentage = mutationPercentage;
            data = new double[size];
            InitializeRandomData();
        }
        
        public StandardGenoPhenoCombination(double[] genes ,int height, int width, int mutationPercentage)
        {
            if (genes.Length != size) throw new ArgumentException("The Genes must have the same lenght as the size");
            this.height = height;
            this.width = width;
            this.mutationPercentage = mutationPercentage;
            data = genes;
        }
        

        private void InitializeRandomData()
        {
            for (int i = 0; i < size; i++)
            {
                data[i] = Random.Range(0f, 1f);
            }
        }

        public void BuildPhenoType()
        {
            map = new int[height, width];
            mappedPositions = new int[size];

            var playerX = (int) (data[0] * width);
            var playerY = (int) (data[1] * height);
            map[playerY, playerX] = 1;
            mappedPositions[0] = playerX;
            mappedPositions[1] = playerY;

            var enemyIndex = 2;
            for (int i = 0; i < 2; i++)
            {
                var enemyX = (int) (data[enemyIndex] * width);
                mappedPositions[enemyIndex] = enemyX;
                enemyIndex++;

                var enemyY = (int) (data[enemyIndex] * height);
                mappedPositions[enemyIndex] = enemyY;
                map[enemyY, enemyX] = 2;
            }
        }


        public void Mutate()
        {
            for (int i = 0; i < size; i++)
            {
                if (Random.Range(0, 100) < mutationPercentage)
                {
                    var random = RandomUtil.GenerateGaussian(0, 0.5);
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

        public int[] MappedPositions => mappedPositions;
        public int[,] Map => map;
        public double[] Data => data;
    }
}