using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;
using Random = System.Random;

namespace Demo.Forest
{
    public class ForestIndividual : AbstractNsga2Individual
    {
        private int areaLength;
        public readonly int[,] Map;
        private int sideLength;
        private Random random;
        private int radius;
        private double mutationPercentage;

        public readonly LongForestArea[] LongForestAreas;
        public readonly RoundForestArea[] RoundForestAreas;
        public Vector2 Start;
        public Vector2 Goal;
        public int[] INTStart;
        public int[] INTGoal;

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
            Map = new int[sideLength, sideLength];

            LongForestAreas = new LongForestArea[numberLongForests];

            for (int i = 0; i < numberLongForests; i++)
            {
                LongForestAreas[i] = new LongForestArea(random);
                DrawLongForest(LongForestAreas[i]);
            }

            RoundForestAreas = new RoundForestArea[numberRoundForests];

            for (int i = 0; i < numberRoundForests; i++)
            {
                RoundForestAreas[i] = new RoundForestArea(random);
                DrawRoundForest(RoundForestAreas[i]);
            }

            Start = new Vector2((float) random.NextDouble(), (float) random.NextDouble());
            DrawStart(Start);
            Goal = new Vector2((float) random.NextDouble(), (float) random.NextDouble());
            DrawEnd(Goal);
        }

        public override IEvolutionaryAlgorithmIndividual MakeOffspring(IEvolutionaryAlgorithmIndividual parent2)
        {
            var child = new ForestIndividual(random, RoundForestAreas.Length, LongForestAreas.Length, radius,
                areaLength, sideLength, mutationPercentage, FitnessFunctions);

            var other = parent2 as ForestIndividual;
            for (var i = 0; i < LongForestAreas.Length; i++)
            {
                if (random.NextDouble() < mutationPercentage)
                {
                    child.LongForestAreas[i] = other.LongForestAreas[i];
                }
                else if (random.NextDouble() < mutationPercentage)
                {
                    child.LongForestAreas[i] = LongForestAreas[i];
                }
            }

            for (int i = 0; i < RoundForestAreas.Length; i++)
            {
                if (random.NextDouble() < mutationPercentage)
                {
                    child.RoundForestAreas[i] = other.RoundForestAreas[i];
                }
                else if (random.NextDouble() < mutationPercentage)
                {
                    child.RoundForestAreas[i] = RoundForestAreas[i];
                }
            }

            if (random.NextDouble() < mutationPercentage)
            {
                child.Start = other.Start;
            }
            else if (random.NextDouble() < mutationPercentage)
            {
                child.Start = Start;
            }

            if (random.NextDouble() < mutationPercentage)
            {
                child.Start = other.Goal;
            }
            else if (random.NextDouble() < mutationPercentage)
            {
                child.Start = Goal;
            }


            return child;
        }

        private void DrawStart(Vector2 Start)
        {
            var x = (int) (Start.x * sideLength);
            var y = (int) (Start.y * sideLength);
            INTStart = new[] {x, y};
            if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                Map[x, y] = 1;
        }

        private void DrawEnd(Vector2 end)
        {
            var x = (int) (end.x * sideLength);
            var y = (int) (end.y * sideLength);
            INTGoal = new[] {x, y};
            if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                Map[x, y] = 2;
        }

        private void DrawLongForest(LongForestArea area)
        {
            var x = (int) (area.X * sideLength);
            var y = (int) (area.Y * sideLength);
            if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                Map[x, y] = 3;

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

                //Debug.Log($"{y} und {x}");
                if (x < sideLength && x >= 0 && y < sideLength && y >= 0)
                    Map[x, y] = 3;
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
            var x1 = (int) (area.X * sideLength);
            var y1 = (int) (area.Y * sideLength);

            var x2 = (int) (area.X * sideLength);
            var y2 = (int) (area.Y * sideLength);

            var x3 = (int) (area.X * sideLength);
            var y3 = (int) (area.Y * sideLength);

            var x4 = (int) (area.X * sideLength);
            var y4 = (int) (area.Y * sideLength);
            var origin = new Vector2(x1, y1);

            if (x1 < sideLength && x1 >= 0 && y1 < sideLength && y1 >= 0)
                Map[x1, y1] = 3;


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
                    if ((new Vector2(x1, yq) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (x1 < sideLength && x1 >= 0 && yq < sideLength && yq >= 0)
                                Map[x1, yq] = 3;
                        }
                    }


                for (var yq = y2; yq < y3; yq++)
                    if ((new Vector2(x2, yq) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (x2 < sideLength && x2 >= 0 && yq < sideLength && yq >= 0)
                                Map[x2, yq] = 3;
                        }
                    }


                for (var xq = x3; xq < x1; xq++)
                    if ((new Vector2(xq, y3) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (xq < sideLength && xq >= 0 && y3 < sideLength && y3 >= 0)
                                Map[xq, y3] = 3;
                        }
                    }


                for (var xq = x4; xq > x2; xq--)
                    if ((new Vector2(xq, y4) - origin).magnitude < radius * area.ExpandFactor)
                    {
                        if (random.NextDouble() < area.HoleFactor)
                        {
                            if (xq < sideLength && xq >= 0 && y4 < sideLength && y4 >= 0)
                                Map[xq, y4] = 3;
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