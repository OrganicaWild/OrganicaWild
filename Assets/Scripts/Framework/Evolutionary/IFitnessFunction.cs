namespace Framework.Evolutionary
{
    public interface IFitnessFunction
    {
        double DetermineFitness(IAlgorithmIndividual individual);
    }
}