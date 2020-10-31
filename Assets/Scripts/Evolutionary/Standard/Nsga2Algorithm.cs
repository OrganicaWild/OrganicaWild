using System;
using System.Collections.Generic;
using System.Linq;
using Evolutionary.Framework;

namespace Evolutionary.Standard
{
    public class Nsga2Algorithm
    {
        private List<INsga2Individual> population;
        private int numberOfOptimizationTargets;

        public int PopulationSize => population.Count;
        private int HalfPopulation => population.Count / 2;

        public void NextGeneration()
        {
           var fronts = NonDominatedSorting();
           var crowdedFronts = fronts.Select(front => CrowdingDistanceSortFront(front)).ToList();

           population = crowdedFronts.SelectMany(x => x).ToList();

           var pt =population.GetRange(HalfPopulation, PopulationSize - 1);

           var random = new Random();
           var comparer = new RankCrowdingComparer();

           while (pt.Count != PopulationSize)
           {
               var first = random.Next(HalfPopulation);
               var second = random.Next(HalfPopulation);

               var parent1 = comparer.Compare(pt[first], pt[second]) >= 0 ? pt[first] : pt[second];
               
               var first2 = random.Next(HalfPopulation);
               var second2 = random.Next(HalfPopulation);

               var parent2 = comparer.Compare(pt[first2], pt[second2]) >= 0 ? pt[first2] : pt[second2];
               
               pt.Add(parent1.MakeOffspring(parent2));
           }

           population = pt;
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
                    if (p.GetDominationCount() == 0)
                    {
                        currentFront.Add(p);
                        p.Rank = frontCounter;
                        pop.Remove(p);
                    }
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

                front[0].SetDistance(double.PositiveInfinity);
                front[front.Count - 1].SetDistance(double.PositiveInfinity);

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