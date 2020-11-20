using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;

namespace Demo
{
    public class EnemyDistanceFitnessFunction : AbstractNsga2FitnessFunction<MyIndividual>
    {
        protected override double DetermineFitness(MyIndividual individual)
        {
            Vector2 averageDistance = new Vector2(0, 0);
            int[] positions = individual.mappedPositions;
            for (int i = 0; i < positions.Length-1; i++)
            {
                averageDistance += new Vector2(positions[i], positions[i++]);
            }

            averageDistance /= positions.Length;
            return averageDistance.magnitude;
        }
    }
}