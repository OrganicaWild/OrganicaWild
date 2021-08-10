using System;
using Framework.Evolutionary.Nsga2;
using UnityEngine;

namespace Demo.Evolutionary.Forest
{
    public class ForestFilledFitnessFunction : AbstractNsga2FitnessFunction<ForestIndividual>
    {
        protected override double DetermineFitness(ForestIndividual individual)
        {
            double value = 0d;
            foreach (ForestIndividual.RoundForestArea roundForest in individual.roundForestAreas)
            {
                value += Math.Abs(0.8 - roundForest.HoleFactor);
            }

            return -value;
        }
    }

    public class SizeForestFitnessFunction : AbstractNsga2FitnessFunction<ForestIndividual>
    {
        protected override double DetermineFitness(ForestIndividual individual)
        {
            double value = 0d;
            foreach (ForestIndividual.RoundForestArea roundForest in individual.roundForestAreas)
            {
                value += roundForest.ExpandFactor;
            }

            value /= individual.roundForestAreas.Length;

            return -value;
        }
    }

    public class FreeSpaceAroundStartAndEndFitnessFunction : AbstractNsga2FitnessFunction<ForestIndividual>
    {
        private readonly int radius;

        public FreeSpaceAroundStartAndEndFitnessFunction(int radius)
        {
            this.radius = radius;
        }

        protected override double DetermineFitness(ForestIndividual individual)
        {
            double value = 0d;

            int xStart = individual.intStart[0];
            int yStart = individual.intStart[1];

            value += FreeSpacesAround(individual, radius, xStart, yStart);

            int xGoal = individual.intGoal[0];
            int yGoal = individual.intGoal[1];

            value += FreeSpacesAround(individual, radius, xGoal, yGoal);
            
            return -value;
        }

        private static int FreeSpacesAround(ForestIndividual individual, int radius, int x, int y)
        {
            int freeSpaces = 0;

            int x1 = x;
            int y1 = y;

            int x2 = x;
            int y2 = y;

            int x3 = x;
            int y3 = y;

            int x4 = x;
            int y4 = y;

            Vector2 origin = new Vector2(x, y);

            int sideLength = individual.map.GetLength(0);

            while (x1 < sideLength || y1 < sideLength || x2 >= 0 || y2 >= 0 || x3 >= 0 || y3 < sideLength ||
                   x4 < sideLength || y4 >= 0)
            {
                x1++;
                y1++;

                x2--;
                y2--;

                x3--;
                y3++;

                x4++;
                y4--;

                for (int yq = y1; yq > y4; yq--)
                    if ((new Vector2(x1, yq) - origin).magnitude < radius)
                    {
                        if (x1 < sideLength && x1 >= 0 && yq < sideLength && yq >= 0)
                            if (individual.map[x1, yq] == 0)
                            {
                                freeSpaces++;
                            }
                    }


                for (int yq = y2; yq < y3; yq++)
                    if ((new Vector2(x2, yq) - origin).magnitude < radius)
                    {
                        if (x2 < sideLength && x2 >= 0 && yq < sideLength && yq >= 0)
                            if (individual.map[x2, yq] == 0)
                            {
                                freeSpaces++;
                            }
                    }


                for (int xq = x3; xq < x1; xq++)
                    if ((new Vector2(xq, y3) - origin).magnitude < radius)
                    {
                        if (xq < sideLength && xq >= 0 && y3 < sideLength && y3 >= 0)
                            if (individual.map[xq, y3] == 0)
                            {
                                freeSpaces++;
                            }
                    }


                for (int xq = x4; xq > x2; xq--)
                    if ((new Vector2(xq, y4) - origin).magnitude < radius)
                    {
                        if (xq < sideLength && xq >= 0 && y4 < sideLength && y4 >= 0)
                            if (individual.map[xq, y4] == 0)
                            {
                                freeSpaces++;
                            }
                    }
            }

            return freeSpaces;
        }
    }
    
    public class DistanceBetweenStartAndEndFitnessFunction : AbstractNsga2FitnessFunction<ForestIndividual>
    {
        protected override double DetermineFitness(ForestIndividual individual)
        {
            float distance = (individual.goal - individual.start).sqrMagnitude;
            if (distance < 0.8)
            {
                return double.MaxValue;
            }
            
            return -distance;
        }
    }
}