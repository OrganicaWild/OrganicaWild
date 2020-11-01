using System.Linq;
using Evolutionary.Framework.Standard.Nsga2;
using Evolutionary.Nsga2;
using UnityEngine;

namespace Evolutionary.Framework.Standard
{
    public class StandardAlgorithmRunner : MonoBehaviour
    {
        public int size;

        public int height;
        public int width;

        public int mutationChance;

        public int generations;

        private Nsga2Algorithm algorithm;
        private IFitnessFunction<StandardGenoPhenoCombination>[] fitnessFunctions;
        private StandardIndividual[] population;

        private void Awake()
        {
            var f1 = new DistanceToBorderFitnessFunction();
            var f2 = new EnemyDistanceFitnessFunction();

            fitnessFunctions = new IFitnessFunction<StandardGenoPhenoCombination>[2];
            fitnessFunctions[0] = f1;
            fitnessFunctions[1] = f2;

            var population = new StandardIndividual[size];
            for (int i = 0; i < size; i++)
            {
                population[i] = new StandardIndividual(height, width, mutationChance, fitnessFunctions);
            }

            this.population = population;
            algorithm = new Nsga2Algorithm(population, fitnessFunctions.Length);
        }

        public void ApplyEvolution()
        {
            for (int generation = 0; generation < generations; generation++)
            {
                Debug.Log($"Generation: {generation}");

                foreach (var individual in population)
                {
                    individual.Representation.BuildPhenoType();
                    individual.EvaluateFitnessFunctions();
                }

                population = algorithm.NextGeneration(population).Cast<StandardIndividual>().ToArray();
            }
        }

        public StandardIndividual[] GetPopulation()
        {
            return population;
        }
    }
}