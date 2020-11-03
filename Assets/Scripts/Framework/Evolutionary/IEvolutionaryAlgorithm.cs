namespace Framework.Evolutionary
{
    public interface IEvolutionaryAlgorithm
    {
        IEvolutionaryAlgorithmIndividual[] NextGeneration();

        IEvolutionaryAlgorithmIndividual[] RunForGenerations(int generations);
    }
}