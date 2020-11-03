namespace Framework.Evolutionary
{
    public interface IEvolutionaryAlgorithm
    {
        IAlgorithmIndividual[] NextGeneration();

        IAlgorithmIndividual[] RunForGenerations(int generations);
    }
}