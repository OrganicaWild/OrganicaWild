using System;
using System.Collections.Generic;
using System.Linq;
using Evolutionary.Framework;

namespace Evolutionary
{
    public class Nsga2Algorithm
    {
        private List<INsga2Individual> population;
        private int numberOfOptimizationTargets;

        private List<List<INsga2Individual>> NonDominatedSorting()
        {
            foreach (var p in population)
            {
                foreach (var q in population)
                {
                    var pEvaluations = p.GetEvaluations();
                    var qEvaluations = q.GetEvaluations();

                    var pDominatesQ = true;
                    for (int i = 0; i < Math.Min(pEvaluations.Length, qEvaluations.Length); i++)
                    {
                        if (pEvaluations[i] > qEvaluations[i])
                        {
                            pDominatesQ = false;
                        }
                    }

                    if (pDominatesQ)
                    {
                        p.AddDominatedIndividual(q);
                        q.IncrementDominationCount();
                    }
                }
            }

            //copy of pop to manipulate
            var pop = population.ToList();
            var fronts = new List<List<INsga2Individual>>();
            var currentFrontList = new List<INsga2Individual>();


            while (pop.Count != 0)
            {
                foreach (var p in pop)
                {
                    if (p.GetDominationCount() == 0)
                    {
                        currentFrontList.Add(p);
                        pop.Remove(p);
                    }
                }

                foreach (var p in currentFrontList)
                {
                    foreach (var q in p.GetDominated())
                    {
                        q.DecrementDominationCount();
                    }
                }

                fronts.Add(currentFrontList);
                currentFrontList = new List<INsga2Individual>();
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

                    p.SetDistance(p.GetDistance() + ((pPlus - pMinus) / (pMax - pMin)));
                }
            }
            
            front.Sort(new DistanceComparer());
            return front;
        }
    }
}