using Demo.Evolutionary;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;

namespace Demo
{
    public class DistanceToBorderFitnessFunction : AbstractNsga2FitnessFunction<MyIndividual>
    {
        protected override double DetermineFitness(MyIndividual individual)
        {
            int[] positions = individual.mappedPositions;
            Vector2 playerPosition = new Vector2(positions[0], positions[1]);

            return playerPosition.magnitude;
        }
    }
}