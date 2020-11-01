using System.Collections.Generic;
using System.Linq;
using Evolutionary.Framework;
using Evolutionary.Framework.Nsga2;
using UnityEngine;


namespace Evolutionary.Standard
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
            
            algorithm = new Nsga2Algorithm(population, fitnessFunctions.Length, population.Length/2);
        }

        public void StartEvolution()
        {
            for (int generation = 0; generation < generations; generation++)
            {
                Debug.Log($"Generation: {generation}");
                
                var pop = GetPopulation();
                foreach (var individual in pop)
                {
                    individual.Representation.BuildPhenoType();
                    individual.EvaluateFitnessFunctions();
                }
                
                algorithm.NextGeneration(pop.ToArray());
            }
        }

        public List<StandardIndividual> GetPopulation()
        {
            var population = algorithm.Population.Cast<StandardIndividual>().ToList();
            return population;
        }
    }
}