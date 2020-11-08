using System;
using Framework.Evolutionary.Nsga2;

namespace Demo
{
    public class RoundnessFitnessFunction : AbstractNsga2FitnessFunction<ForestIndividual>
    {
        protected override double DetermineFitness(ForestIndividual individual)
        {
            var value = 0d;
            foreach (var roundForest in individual.RoundForestAreas)
            {
                value += roundForest.HoleFactor;
            }

            value /= individual.RoundForestAreas.Length;

            return Math.Abs(value - 0.3);
        }
    }
}