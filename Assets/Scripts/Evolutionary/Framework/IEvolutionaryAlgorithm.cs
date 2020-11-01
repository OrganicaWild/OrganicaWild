using Evolutionary.Nsga2;

namespace Evolutionary.Framework
{
    public interface IEvolutionaryAlgorithm
    {
        IAlgorithmIndividual[] NextGeneration(IAlgorithmIndividual[] currentPopulation);
    }
}