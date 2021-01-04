using System;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo
{
    public class StandardIndividual2 : AbstractNsga2Individual
    {
        public readonly int[,] map = new int[100, 100];
        internal Vector2 StartLocation { get; }
        internal Vector2 EndLocation { get; }

        public StandardIndividual2(Vector2 start, Vector2 end, IFitnessFunction[] fitnessFunctions)
            : base(fitnessFunctions)
        {
            StartLocation = start;
            EndLocation = end;
            BuildPhenoType();
        }

        private void BuildPhenoType()
        {
            int playerX = (int) (StartLocation[0] * 100);
            int playerY = (int) (StartLocation[1] * 100);
            map[playerY, playerX] = 1;

            playerX = (int) (EndLocation[0] * 100);
            playerY = (int) (EndLocation[1] * 100);
            map[playerY, playerX] = 2;
        }
        
        public override IEvolutionaryAlgorithmIndividual MakeOffspring(IEvolutionaryAlgorithmIndividual parent2)
        {
            StandardIndividual2 other = parent2 as StandardIndividual2;
            if (other == null)
                throw new NullReferenceException($"Give a Nsga2 Individual of Type {nameof(StandardIndividual2)}");
            if (Random.value < 0.5f)
                return new StandardIndividual2(
                    new Vector2(StartLocation.x, StartLocation.y),
                    new Vector2(EndLocation.x, EndLocation.y), FitnessFunctions);

            return new StandardIndividual2(
                new Vector2(StartLocation.x, StartLocation.y),
                new Vector2(EndLocation.x, EndLocation.y), FitnessFunctions);
        }
    }
}