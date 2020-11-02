using System.Linq;
using Framework.Evolutionary.Standard.Nsga2;
using UnityEngine;

namespace Framework.Evolutionary.Standard
{
    public class StandardAlgorithmRunner2 : MonoBehaviour
    {
        private IEvolutionaryAlgorithm algorithm;
        public int generations;

        public int height;

        public int mutationChance;
        private StandardIndividual2[] population;
        public int size;
        public int width;

        private void Awake()
        {
            var f1 = new DistanceToBorderFitnessFunction();
            var f2 = new EnemyDistanceFitnessFunction();

            population = new StandardIndividual2[size];
            for (var i = 0; i < size; i++)
                population[i] = new StandardIndividual2(
                    new Vector2(Random.value, Random.value),
                    new Vector2(Random.value, Random.value));

            algorithm = new Nsga2Algorithm(population, 3);
        }

        public void ApplyEvolution()
        {
            for (var generation = 0; generation < generations; generation++)
            {
                Debug.Log($"Generation: {generation}");

                foreach (var individual in population)
                {
                    individual.Representation.BuildPhenoType();
                    individual.EvaluateFitnessFunctions();
                }

                population = algorithm.NextGeneration(population).Cast<StandardIndividual2>().ToArray();
            }
        }

        public StandardIndividual2[] GetPopulation()
        {
            return population;
        }
    }
}