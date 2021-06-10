namespace Framework.Evolutionary
{
    /// <summary>
    /// Interface that needs to be implemented, when creating a new fitness function for testing an individual.
    /// It is encouraged to use AbstractNsga2FitnessFunction when implementing fitness functions for
    /// a concrete implementation of a INsga2Individual.
    ///
    /// Only use this interface when using a different IEvolutionaryAlgorithm.
    /// When implementing your own evolutionary algorithm it is also recommended to build an abstract base class
    /// after the model of AbstractNsga2FitnessFunction.
    /// </summary>
    public interface IFitnessFunction
    {
        double DetermineFitness(IEvolutionaryAlgorithmIndividual individual);
    }
}