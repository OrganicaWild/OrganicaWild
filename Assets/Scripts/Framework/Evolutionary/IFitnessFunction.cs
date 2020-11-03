namespace Framework.Evolutionary
{
    public interface IFitnessFunction
    {
        double DetermineFitness(IEvolutionaryAlgorithmIndividual individual);
    }
}