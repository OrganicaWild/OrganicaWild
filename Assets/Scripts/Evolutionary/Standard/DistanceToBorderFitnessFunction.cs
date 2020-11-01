
using Evolutionary.Framework;
using UnityEngine;

namespace Evolutionary.Standard
{
    public class DistanceToBorderFitnessFunction : IFitnessFunction<StandardGenoPhenoCombination>
    {
        public double DetermineFitness(StandardGenoPhenoCombination phenoType)
        {
            var positions = phenoType.MappedPositions;
            var playerPosition = new Vector2(positions[0], positions[1]);

            return playerPosition.magnitude;
        }
    }
}