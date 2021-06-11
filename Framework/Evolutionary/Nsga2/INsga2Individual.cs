using System.Collections.Generic;

namespace Framework.Evolutionary.Nsga2
{
    /// <summary>
    /// Interface that has to be implemented by an Individual so it can be used inside of the Nsga-2 Algorithm
    ///
    /// For further explanation on certain values like Rank, Crowing Distance, Front and Domination Count see this paper:
    /// https://ieeexplore.ieee.org/document/996017
    /// </summary>
    public interface INsga2Individual : IEvolutionaryAlgorithmIndividual
    {
        /// <summary>
        /// rank of the individual.
        /// This corresponds to the front which the individual is being ranked in by the Algorithm.
        /// 0 is the best rank possible or also called the pareto front. Decreases towards positive infinity.
        /// </summary>
        int Rank { get; set; }

        /// <summary>
        /// Crowding Distance of the given individual.
        /// This is set by the Algorithm during the Crowding Sorting.
        /// A bigger value is better.
        /// </summary>
        double Crowding { get; set; }
       
        /// <summary>
        /// Domination Count of this individual. 
        /// </summary>
        int DominationCount { get; set; }

        /// <summary>
        /// Adds a dominated Individual to this individual.
        /// Used by Algorithm during Non-dominated Sorting.
        /// </summary>
        /// <param name="dominated">dominated individual</param>
        void AddDominatedIndividual(INsga2Individual dominated);

        /// <summary>
        /// Gets all the other individuals which are being dominated by this individual.
        /// </summary>
        /// <returns>List of all dominated individuals</returns>
        IList<INsga2Individual> GetDominated();
        
        
    }
}