using System.Collections.Generic;

namespace Framework.Evolutionary
{
    /// <summary>
    /// Interface for any sort of evolutionary optimization algorithm.
    /// </summary>
    public interface IEvolutionaryAlgorithm
    {
        /// <summary>
        /// Should advance only one generation.
        /// </summary>
        /// <returns>population after advancing one generation</returns>
        IEvolutionaryAlgorithmIndividual[] NextGeneration();

        /// <summary>
        /// Advance a given number of generations.
        /// </summary>
        /// <param name="generations">number of generations to advance</param>
        /// <returns>population after advancing the given number of generations</returns>
        IEnumerable<IEvolutionaryAlgorithmIndividual> RunForGenerations(int generations);
    }
}