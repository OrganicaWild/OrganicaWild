using System;
using System.Collections.Generic;
using Demo;

namespace Framework.Evolutionary.Nsga2
{
    /// <summary>
    /// Abstract base class for implementing new individuals that can be used inside of the NSGA-2 Algorithm
    /// It is encouraged to use this abstract base class instead of the
    /// underlying INsga2Individual interface for implementing your own types.
    /// It reduces your implementation from many book keeping operations that the algorithm needs to function.
    /// 
    /// The only downside this implementation is that you need to specify all fitness functions as an argument to the constructor.
    /// So you can not change the fitness functions of an individual after the constructor.
    /// However, this is an edge case and will mostly not be done.
    ///
    /// For further explanation on certain values like Rank, Crowing Distance, Front and Domination Count see this paper:
    /// https://ieeexplore.ieee.org/document/996017
    /// </summary>
    public abstract class AbstractNsga2Individual : INsga2Individual
    {
        /// <summary>
        /// Indicates to what front the individual belongs.
        /// </summary>
        public int Rank { get; set; }
        
        /// <summary>
        /// Crowding Distance
        /// </summary>
        public double Crowding { get; set; }
        
        /// <summary>
        /// Number of individuals this individual dominated by.
        /// </summary>
        public int DominationCount { get; set; }

        /// <summary>
        /// Individuals that are being dominated by this individual.
        /// </summary>
        private IList<INsga2Individual> dominatedIndividuals = new List<INsga2Individual>();

        /// <summary>
        /// Fitness Functions that the individual will be tested on by the Nsga2 Algorithm.
        /// </summary>
        public readonly IFitnessFunction[] FitnessFunctions;

        /// <summary>
        /// results of the FitnessFunctions after EvaluateFitness() has been called by the Algorithm.
        /// </summary>
        private readonly double[] fitnessResults;

        /// <summary>
        /// Constructor enforcing the passing of fitness functions.
        /// </summary>
        /// <param name="fitnessFunctions">fitness functions</param>
        /// <exception cref="ArgumentNullException">if fitnessFunctions is null</exception>
        protected AbstractNsga2Individual(IFitnessFunction[] fitnessFunctions)
        {
            if (fitnessFunctions is null)
                throw new ArgumentNullException($"{nameof(fitnessFunctions)} is null.");
            
            FitnessFunctions = fitnessFunctions;
            fitnessResults = new double[fitnessFunctions.Length];
        }

        /// <summary>
        /// Runs all fitness functions and stores the values in the fitnessResults array.
        /// </summary>
        public void EvaluateFitness()
        {
            for (var index = 0; index < FitnessFunctions.Length; index++)
            {
                var f = FitnessFunctions[index];
                fitnessResults[index] = f.DetermineFitness(this);
            }
        }

        /// <summary>
        /// Get number of optimization targets (or also called number of fitness functions)
        /// </summary>
        /// <returns>number of fitness functions</returns>
        public int GetNumberOfFitnessFunctions()
        {
            return FitnessFunctions.Length;
        }

        /// <summary>
        /// Get the value of a certain fitness function
        /// </summary>
        /// <param name="index">index of fitness function</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">if index is not in range of fitness functions</exception>
        public double GetOptimizationTarget(int index)
        {
            if (index >= FitnessFunctions.Length || index < 0)
            {
                throw new IndexOutOfRangeException(
                    $"The Index is out of range for the Optimization Targets. Valid Range is {0} to {FitnessFunctions.Length - 1} ");
            }

            return fitnessResults[index];
        }
        
        /// <summary>
        /// Produce offspring of two INsga2Individuals. Implement this method according to what your Individuals look like.
        /// It is safe to cast parent2 back to the concrete Implementation, since all Individuals inside of an array must have the same Assigned type.
        /// </summary>
        /// <param name="parent2">second parent</param>
        /// <returns>New Instance of a child</returns>
        public abstract INsga2Individual MakeOffspring(INsga2Individual parent2);

        /// <summary>
        /// Reset all values for the next generation.
        /// This is executed prior to a new generation. So Rank, Crowding etc. can be read out in between generations.
        /// </summary>
        public void PrepareForNextGeneration()
        {
            DominationCount = 0;
            Rank = 0;
            Crowding = 0;
            dominatedIndividuals = new List<INsga2Individual>();
        }
        
        /// <summary>
        /// Adds an individual to the list of individuals this individual dominates.
        /// </summary>
        /// <param name="dominated">individual that is dominated by this individual</param>
        public void AddDominatedIndividual(INsga2Individual dominated)
        {
            dominatedIndividuals.Add(dominated);
        }

        /// <summary>
        /// Get a list of individuals dominated by this individual.
        /// </summary>
        /// <returns>reference to the list of dominated individuals</returns>
        public IList<INsga2Individual> GetDominated()
        {
            return dominatedIndividuals;
        }
    }
}