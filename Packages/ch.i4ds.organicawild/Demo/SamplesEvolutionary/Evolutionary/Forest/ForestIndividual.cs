using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;
using Random = System.Random;

namespace Demo.Evolutionary.Forest
{
    public class ForestIndividual : AbstractNsga2Individual
    {
        private readonly int areaLength;
        public readonly int[,] map;
        private readonly int sideLength;
        private readonly Random random;
        private readonly int radius;
        private readonly double mutationPercentage;

        private readonly LongForestArea[] longForestAreas;
        public readonly RoundForestArea[] roundForestAreas;
        public Vector2 start;
        public Vector2 goal;
        public int[] intStart;
        public int[] intGoal;

        public ForestIndividual(Random random, int numberRoundForests, int numberLongForests, int radius,
            int areaLength, int sideLength, double mutationPercentage
            , IFitnessFunction[] fitnessFunctions) : base(
            fitnessFunctions)
        {
            this.mutationPercentage = mutationPercentage;
            this.radius = radius;
            this.random = random;
            this.sideLength = sideLength;
            this.areaLength = areaLength;
            map = new int[sideLength, sideLength];

            longForestAreas = new LongForestArea[numberLongForests];

            for (int i = 0; i < numberLongForests; i++)
            {
                longForestAreas[i] = new LongForestArea(random);
                DrawLongForest(longForestAreas[i]);
            }

            roundForestAreas = new RoundForestArea[numberRoundForests];

            for (int i = 0; i < numberRoundForests; i++)
            {
                roundForestAreas[i] = new RoundForestArea(random);
                DrawRoundForest(roundForestAreas[i]);
            }

            start = new Vector2((float) random.NextDouble(), (float) random.NextDouble());
            DrawStart();
            goal = new Vector2((float) random.NextDouble(), (float) random.NextDouble());
            DrawGoal();
        }

        public override INsga2Individual MakeOffspring(INsga2Individual parent2)
        {
            ForestIndividual child = new ForestIndividual(random, roundForestAreas.Length, longForestAreas.Length, radius,
                areaLength, sideLength, mutationPercentage, fitnessFunctions);

            ForestIndividual other = parent2 as ForestIndividual;
            for (int i = 0; i < longForestAreas.Length; i++)
            {
                if (random.NextDouble() < mutationPercentage)
                {
                    child.longForestAreas[i] = other.longForestAreas[i];
                }
                else if (random.NextDouble() < mutationPercentage)
                {
                    child.longForestAreas[i] = longForestAreas[i];
                }
            }

            for (int i = 0; i < roundForestAreas.Length; i++)
            {
                if (random.NextDouble() < mutationPercentage)
                {
                    child.roundForestAreas[i] = other.roundForestAreas[i];
                }
                else if (random.NextDouble() < mutationPercentage)
                {
                    child.roundForestAreas[i] = roundForestAreas[i];
                }
            }

            if (random.NextDouble() < mutationPercentage)
            {
                child.start = other.start;
            }
            else if (random.NextDouble() < mutationPercentage)
            {
                child.start = start;
            }

            if (random.NextDouble() < mutationPercentage)
            {
                child.start = other.goal;
            }
            else if (random.NextDouble() < mutationPercentage)
            {
                child.start = goal;
            }


            return child;
        }

        private void DrawStart()
        {
            int x = (int) (start.x * sideLength);
            int y = (int) (start.y * sideLength);
            intStart = new[] {x, y};
            if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                map[x, y] = 1;
        }

        private void DrawGoal()
        {
            int x = (int) (goal.x * sideLength);
            int y = (int) (goal.y * sideLength);
            intGoal = new[] {x, y};
            if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                map[x, y] = 2;
        }

        private void DrawLongForest(LongForestArea area)
        {
            int x = (int) (area.X * sideLength);
            int y = (int) (area.Y * sideLength);
            if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                map[x, y] = 3;

            for (int i = 0; i < area.Length * areaLength; i++)
            {
                if (area.TurnLeft < random.NextDouble())
                {
                    y++;
                }
                else if (area.TurnRight < random.NextDouble())
                {
                    y--;
                }
                else
                {
                    x++;
                }
                
                if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                    map[x, y] = 3;
            }
        }

        public class LongForestArea
        {
            public double X { get; }
            public double Y { get; }
            public double TurnLeft { get; }
            public double TurnRight { get; }
            public double Length { get; }

            public LongForestArea(Random random)
            {
                X = random.NextDouble();
                Y = random.NextDouble();
                TurnLeft = random.NextDouble();
                TurnRight = random.NextDouble();
                Length = random.NextDouble();
            }
        }

        private void DrawRoundForest(RoundForestArea area)
        {
            int x1 = (int) (area.X * sideLength);
            int y1 = (int) (area.Y * sideLength);

            int x2 = (int) (area.X * sideLength);
            int y2 = (int) (area.Y * sideLength);

            int x3 = (int) (area.X * sideLength);
            int y3 = (int) (area.Y * sideLength);

            int x4 = (int) (area.X * sideLength);
            int y4 = (int) (area.Y * sideLength);
            Vector2 origin = new Vector2(x1, y1);

            if (x1 < sideLength && x1 >= 0 && y1 < sideLength && y1 >= 0)
                map[x1, y1] = 3;


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
                    if ((new Vector2(x1, yq) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (x1 < sideLength && x1 >= 0 && yq < sideLength && yq >= 0)
                                map[x1, yq] = 3;
                        }
                    }


                for (int yq = y2; yq < y3; yq++)
                    if ((new Vector2(x2, yq) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (x2 < sideLength && x2 >= 0 && yq < sideLength && yq >= 0)
                                map[x2, yq] = 3;
                        }
                    }


                for (int xq = x3; xq < x1; xq++)
                    if ((new Vector2(xq, y3) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (xq < sideLength && xq >= 0 && y3 < sideLength && y3 >= 0)
                                map[xq, y3] = 3;
                        }
                    }


                for (int xq = x4; xq > x2; xq--)
                    if ((new Vector2(xq, y4) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (xq < sideLength && xq >= 0 && y4 < sideLength && y4 >= 0)
                                map[xq, y4] = 3;
                        }
                    }
            }
        }

        public class RoundForestArea
        {
            public double X { get; }
            public double Y { get; }
            public double ExpandFactor { get; }
            public double HoleFactor { get; }

            public RoundForestArea(Random random)
            {
                X = random.NextDouble();
                Y = random.NextDouble();
                ExpandFactor = random.NextDouble();
                HoleFactor = random.NextDouble();
            }
        }
    }
}