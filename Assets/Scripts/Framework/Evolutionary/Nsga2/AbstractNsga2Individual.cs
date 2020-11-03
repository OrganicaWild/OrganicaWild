using System;
using System.Collections.Generic;
using Demo;

namespace Framework.Evolutionary.Nsga2
{
    public abstract class AbstractNsga2Individual : INsga2Individual
    {
        public int Rank { get; set; }
        public double Crowding { get; set; }
        public int DominationCount { get; set; }

        private IList<INsga2Individual> dominatedIndividuals = new List<INsga2Individual>();

        public readonly IFitnessFunction[] FitnessFunctions;

        private readonly double[] fitnessResults;

        protected AbstractNsga2Individual(IFitnessFunction[] fitnessFunctions)
        {
            if (fitnessFunctions is null)
                throw new ArgumentNullException($"{nameof(fitnessFunctions)} is null.");
            
            FitnessFunctions = fitnessFunctions;
            fitnessResults = new double[fitnessFunctions.Length];
        }

        public void EvaluateFitness()
        {
            for (var index = 0; index < FitnessFunctions.Length; index++)
            {
                var f = FitnessFunctions[index];
                fitnessResults[index] = f.DetermineFitness(this);
            }
        }

        public int GetNumberOfFitnessFunctions()
        {
            return FitnessFunctions.Length;
        }

        public double GetOptimizationTarget(int index)
        {
            if (index >= FitnessFunctions.Length || index < 0)
            {
                throw new IndexOutOfRangeException(
                    $"The Index is out of range for the Optimization Targets. Valid Range is {0} to {FitnessFunctions.Length - 1} ");
            }

            return fitnessResults[index];
        }

        public abstract INsga2Individual MakeOffspring(INsga2Individual parent2);

        public void PrepareForNextGeneration()
        {
            DominationCount = 0;
            Rank = 0;
            Crowding = 0;
            dominatedIndividuals = new List<INsga2Individual>();
        }


        public void AddDominatedIndividual(INsga2Individual dominated)
        {
            dominatedIndividuals.Add(dominated);
        }

        public IList<INsga2Individual> GetDominated()
        {
            return dominatedIndividuals;
        }
    }
}