using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Evolutionary.Framework.Nsga2
{
    public class Nsga2Algorithm
    {
        private INsga2Individual[] population;
        private int numberOfOptimizationTargets;

        private int lambda;
        private int PopulationSize => population.Length;
        private int HalfPopulation => population.Length / 2;

        public Nsga2Algorithm(INsga2Individual[] population, int numberOfOptimizationTargets, int lambda)
        {
            this.population = population;
            this.numberOfOptimizationTargets = numberOfOptimizationTargets;
            this.lambda = lambda;
        }

        public void NextGeneration(INsga2Individual[] currentPopulation)
        {
            this.population = currentPopulation;
            foreach (var p in population)
            {
                p.CleanUp();
            }
            
            var fronts = NonDominatedSorting();
            var crowdedFronts = fronts.Select(front => CrowdingDistanceSortFront(front)).ToList();

            var newPopulation = crowdedFronts.SelectMany(x => x).ToArray();

            var random = new Random();
            var comparer = new RankCrowdingComparer();

            for (int i = 0; i < PopulationSize; i++)
            {
                if (i <= HalfPopulation)
                {
                    population[i] = newPopulation[i];
                }
                else
                {
                    var first = random.Next(HalfPopulation - 1);
                    var second = random.Next(HalfPopulation - 1);

                    var parent1 = comparer.Compare(newPopulation[first], newPopulation[second]) >= 0
                        ? newPopulation[first]
                        : newPopulation[second];

                    var first2 = random.Next(HalfPopulation - 1);
                    var second2 = random.Next(HalfPopulation - 1);

                    var parent2 = comparer.Compare(newPopulation[first2], newPopulation[second2]) >= 0
                        ? newPopulation[first2]
                        : newPopulation[second2];

                    population[i] = parent1.MakeOffspring(parent2);
                }
            }
        }

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
                        q.IncrementDominationCount();
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
                    if (p.GetDominationCount() <= 0)
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
                        q.DecrementDominationCount();
                    }
                }

                fronts.Add(currentFront);
                currentFront = new List<INsga2Individual>();
                frontCounter++;
            }

            return fronts;
        }

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

        public INsga2Individual[] Population => population;
    }
}