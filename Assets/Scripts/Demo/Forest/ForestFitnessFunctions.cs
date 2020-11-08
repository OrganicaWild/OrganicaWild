using System;
using Framework.Evolutionary.Nsga2;
using UnityEngine;

namespace Demo.Forest
{
    public class ForestFilledFitnessFunction : AbstractNsga2FitnessFunction<ForestIndividual>
    {
        protected override double DetermineFitness(ForestIndividual individual)
        {
            var value = 0d;
            foreach (var roundForest in individual.RoundForestAreas)
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
            var value = 0d;
            foreach (var roundForest in individual.RoundForestAreas)
            {
                value += roundForest.ExpandFactor;
            }

            value /= individual.RoundForestAreas.Length;

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
            var value = 0d;

            var xStart = individual.INTStart[0];
            var yStart = individual.INTStart[1];

            value += FreeSpacesAround(individual, radius, xStart, yStart);

            var xGoal = individual.INTGoal[0];
            var yGoal = individual.INTGoal[1];

            value += FreeSpacesAround(individual, radius, xGoal, yGoal);
            
            return -value;
        }

        private static int FreeSpacesAround(ForestIndividual individual, int radius, int x, int y)
        {
            var freeSpaces = 0;

            var x1 = x;
            var y1 = y;

            var x2 = x;
            var y2 = y;

            var x3 = x;
            var y3 = y;

            var x4 = x;
            var y4 = y;

            var origin = new Vector2(x, y);

            var sideLength = individual.Map.GetLength(0);

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

                for (var yq = y1; yq > y4; yq--)
                    if ((new Vector2(x1, yq) - origin).magnitude < radius)
                    {
                        if (x1 < sideLength && x1 >= 0 && yq < sideLength && yq >= 0)
                            if (individual.Map[x1, yq] == 0)
                            {
                                freeSpaces++;
                            }
                    }


                for (var yq = y2; yq < y3; yq++)
                    if ((new Vector2(x2, yq) - origin).magnitude < radius)
                    {
                        if (x2 < sideLength && x2 >= 0 && yq < sideLength && yq >= 0)
                            if (individual.Map[x2, yq] == 0)
                            {
                                freeSpaces++;
                            }
                    }


                for (var xq = x3; xq < x1; xq++)
                    if ((new Vector2(xq, y3) - origin).magnitude < radius)
                    {
                        if (xq < sideLength && xq >= 0 && y3 < sideLength && y3 >= 0)
                            if (individual.Map[xq, y3] == 0)
                            {
                                freeSpaces++;
                            }
                    }


                for (var xq = x4; xq > x2; xq--)
                    if ((new Vector2(xq, y4) - origin).magnitude < radius)
                    {
                        if (xq < sideLength && xq >= 0 && y4 < sideLength && y4 >= 0)
                            if (individual.Map[xq, y4] == 0)
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
            var distance = (individual.Goal - individual.Start).sqrMagnitude;
            if (distance < 0.8)
            {
                return double.MaxValue;
            }
            
            return -distance;
        }
    }
}