﻿using System;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.Evolutionary
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
            BuildPhenotype();
        }

        private void BuildPhenotype()
        {
            int playerX = (int) (StartLocation[0] * 100);
            int playerY = (int) (StartLocation[1] * 100);
            map[playerY, playerX] = 1;

            playerX = (int) (EndLocation[0] * 100);
            playerY = (int) (EndLocation[1] * 100);
            map[playerY, playerX] = 2;
        }
        
        public override INsga2Individual MakeOffspring(INsga2Individual parent2)
        {
            if (!(parent2 is StandardIndividual2))
                throw new NullReferenceException($"Give a Nsga2 Individual of Type {nameof(StandardIndividual2)}");
            if (Random.value < 0.5f)
                return new StandardIndividual2(
                    new Vector2(StartLocation.x, StartLocation.y),
                    new Vector2(EndLocation.x, EndLocation.y), fitnessFunctions);

            return new StandardIndividual2(
                new Vector2(StartLocation.x, StartLocation.y),
                new Vector2(EndLocation.x, EndLocation.y), fitnessFunctions);
        }
    }
}