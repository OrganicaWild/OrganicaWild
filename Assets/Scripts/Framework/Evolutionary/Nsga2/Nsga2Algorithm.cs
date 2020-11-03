using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace Framework.Evolutionary.Nsga2
{
    /// <summary>
    /// NSGA-2 Algorithm implementation for multi-variate optimization.
    ///
    /// For more information on this algorithm visit:
    /// https://ieeexplore.ieee.org/document/996017
    /// </summary>
    public class Nsga2Algorithm : IEvolutionaryAlgorithm
    {
        /// <summary>
        /// Population for the algorithm to work with. The individual objects in the array are reassigned during execution of the algorithm.
        /// </summary>
        private readonly INsga2Individual[] population;

        /// <summary>
        /// Number of Optimization Dimensions the Algorithm optimizes for.
        /// In the context here. Every optimization target maps to a value of one fitness function of the Individuals. 
        /// </summary>
        private readonly int numberOfOptimizationTargets;


        /// <summary>
        /// Create an NSGA-2 instance with the initial population of individuals.
        /// The Individuals must implement the INsga2Individual interface, that they can be used with the NSGA-2 Algorithm.
        /// To reduce bookkeeping and for easier use it is recommended to use the AbstractNsga2Individual as a base for your own INsga2Individual implementations.
        /// </summary>
        /// <param name="initialPopulation">Array of instances of a single concrete implementation of the INsga2Individual interface</param>
        /// <exception cref="FitnessFunctionNumberMismatchException"></exception>
        public Nsga2Algorithm(INsga2Individual[] initialPopulation)
        {
            population = initialPopulation;

            var numberOfFitnessFunctions = initialPopulation[0].GetNumberOfFitnessFunctions();
            for (var index = 1; index < initialPopulation.Length; index++)
            {
                var p = initialPopulation[index];
                if (numberOfFitnessFunctions != p.GetNumberOfFitnessFunctions())
                {
                    throw new FitnessFunctionNumberMismatchException(
                        $"{initialPopulation}s contained Individuals do not share the same number of fitness functions.");
                }
            }

            numberOfOptimizationTargets = numberOfFitnessFunctions;
        }

        /// <summary>
        /// Advance one generation in the optimization process.
        /// </summary>
        /// <returns>population after the generation. Can be casted back to the original type you passed the constructor.</returns>
        public IEvolutionaryAlgorithmIndividual[] NextGeneration()
        {
            foreach (var p in population)
            {
                p.PrepareForNextGeneration();
                p.EvaluateFitness();
            }

            var fronts = NonDominatedSorting();
            var crowdedFronts = fronts.Select(CrowdingDistanceSortFront).ToList();

            var newPopulation = crowdedFronts.SelectMany(x => x).ToArray();

            var random = new Random();
            var comparer = new RankCrowdingComparer();

            var halfPopulation = population.Length / 2;

            for (int i = 0; i < population.Length; i++)
            {
                if (i <= halfPopulation)
                {
                    population[i] = newPopulation[i];
                }
                else
                {
                    var first = random.Next(halfPopulation - 1);
                    var second = random.Next(halfPopulation - 1);

                    var parent1 = comparer.Compare(newPopulation[first], newPopulation[second]) >= 0
                        ? newPopulation[first]
                        : newPopulation[second];

                    var first2 = random.Next(halfPopulation - 1);
                    var second2 = random.Next(halfPopulation - 1);

                    var parent2 = comparer.Compare(newPopulation[first2], newPopulation[second2]) >= 0
                        ? newPopulation[first2]
                        : newPopulation[second2];

                    population[i] = parent1.MakeOffspring(parent2);
                }
            }

            return population;
        }

        /// <summary>
        /// Advance a certain number of generations
        /// </summary>
        /// <param name="generations">Number indicating how many generations to advance</param>
        /// <returns>Population after advance that many generations</returns>
        public IEvolutionaryAlgorithmIndividual[] RunForGenerations(int generations)
        {
            for (int i = 0; i < generations; i++)
            {
                NextGeneration();
            }

            return population;
        }

        /// <summary>
        /// Sort the population with non-dominated sorting.
        /// </summary>
        /// <returns>Population sorted into fronts. Front 0 is the pareto front.</returns>
        private List<List<INsga2Individual>> NonDominatedSorting()
        {
            foreach (var p in population)
            {
                foreach (var q in population)
                {
                    var firstBlock = true;
                    var secondBlock = false;
                    for (int o = 0; o < numberOfOptimizationTargets; o++)
                    {
                        var pTarget = p.GetOptimizationTarget(o);
                        var qTarget = q.GetOptimizationTarget(o);
                        if (pTarget > qTarget)
                        {
                            firstBlock = false;
                        }

                        if (pTarget < qTarget)
                        {
                            secondBlock = true;
                        }
                    }

                    if (firstBlock & secondBlock)
                    {
                        p.AddDominatedIndividual(q);
                        q.DominationCount++;
                    }
                }
            }

            //copy of pop to manipulate
            var pop = population.ToList();
            var fronts = new List<List<INsga2Individual>>();
            var currentFront = new List<INsga2Individual>();
            var frontCounter = 0;

            while (pop.Count != 0)
            {
                foreach (var p in pop)
                {
                    if (p.DominationCount <= 0)
                    {
                        currentFront.Add(p);
                        p.Rank = frontCounter;
                        //pop.Remove(p);
                    }
                }

                foreach (var p in currentFront)
                {
                    pop.Remove(p);
                }

                foreach (var p in currentFront)
                {
                    foreach (var q in p.GetDominated())
                    {
                        q.DominationCount--;
                    }
                }

                fronts.Add(currentFront);
                currentFront = new List<INsga2Individual>();
                frontCounter++;
            }

            return fronts;
        }

        /// <summary>
        /// Sort a front by crowding distance
        /// </summary>
        /// <param name="front">Front to be sorted</param>
        /// <returns>Sorted Front</returns>
        private List<INsga2Individual> CrowdingDistanceSortFront(List<INsga2Individual> front)
        {
            for (var o = 0; o < numberOfOptimizationTargets; o++)
            {
                front.Sort(new OptimizationTargetComparer(o));

                front[0].Crowding = double.PositiveInfinity;
                front[front.Count - 1].Crowding = double.PositiveInfinity;

                var pMax = front[0].GetOptimizationTarget(o);
                var pMin = front[front.Count - 1].GetOptimizationTarget(o);

                for (var index = 1; index < front.Count - 1; index++)
                {
                    var p = front[index];
                    var pMinus = front[index - 1].GetOptimizationTarget(o);
                    var pPlus = front[index + 1].GetOptimizationTarget(o);

                    p.Crowding += (pPlus - pMinus) / (pMax - pMin);
                }
            }

            front.Sort(new DistanceComparer());
            return front;
        }
    }
}