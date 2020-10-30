using System.Collections.Generic;
using Evolutionary.Framework;
using UnityEngine;

namespace Evolutionary
{
    public class Individual : IIndividual<RandomVectorGenoPhenoCombination>, INsga2Individual
    {

        private int dominationCount;
        private List<INsga2Individual> dominatedIndividuals;

        private RandomVectorGenoPhenoCombination representation;
        
        private List<IFitnessFunction<RandomVectorGenoPhenoCombination>> fitnessFunctions;
        private double[] evaluations;

        private double distance;
        
        public Individual()
        {
            dominationCount = 0;
            dominatedIndividuals = new List<INsga2Individual>();
        }
        
        public void EvaluatePhenoType()
        {
            evaluations = new double[fitnessFunctions.Count];
            
            for (var index = 0; index < fitnessFunctions.Count; index++)
            {
                var f = fitnessFunctions[index];
                evaluations[index] = f.Apply(representation);
            }
        }

        public void AddFitnessFunction(IFitnessFunction<RandomVectorGenoPhenoCombination> fitnessFunction)
        {
            fitnessFunctions.Add(fitnessFunction);
        }

        public void AddDominatedIndividual(INsga2Individual dominated)
        {
            dominatedIndividuals.Add(dominated);
        }

        public List<INsga2Individual> GetDominated()
        {
            return dominatedIndividuals;
        }

        public void IncrementDominationCount()
        {
            dominationCount++;
        }

        public void DecrementDominationCount()
        {
            dominationCount--;
            Debug.Assert(dominationCount >= 0);
        }

        public int GetDominationCount()
        {
            return dominationCount;
        }

        public double[] GetEvaluations()
        {
            return evaluations;
        }

        public double GetOptimizationTarget(int index)
        {
            return evaluations[index];
        }

        public void SetDistance(double distance)
        {
            this.distance = distance;
        }

        public double GetDistance()
        {
           return distance;
        }
    }
}