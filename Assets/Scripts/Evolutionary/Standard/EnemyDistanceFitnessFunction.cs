using Evolutionary.Framework;
using UnityEngine;

namespace Evolutionary.Standard
{
    public class EnemyDistanceFitnessFunction : IFitnessFunction<StandardGenoPhenoCombination>
    {
        public double DetermineFitness(StandardGenoPhenoCombination phenoType)
        {
            var averageDistance = new Vector2(0, 0);
            var positions = phenoType.MappedPositions;
            for (int i = 0; i < positions.Length-1; i++)
            {
                averageDistance += new Vector2(positions[i], positions[i++]);
            }

            averageDistance /= positions.Length;
            return averageDistance.magnitude;
        }
    }
}