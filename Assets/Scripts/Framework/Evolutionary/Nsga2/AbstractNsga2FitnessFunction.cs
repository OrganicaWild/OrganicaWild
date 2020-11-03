namespace Framework.Evolutionary.Nsga2
{
    /// <summary>
    /// Abstract class to generify IFitnessFunction Interface.
    /// It is encouraged to use this abstract class to implement a new FitnessFunction instead of the raw IFitnessFunction interface,
    /// if the fitness function is only used in the NSGA-2 Algorithm.
    /// 
    /// Only use the IFitnessFunction interface if the fitness function should be used in in different evolutionary algorithms.
    /// </summary>
    /// <typeparam name="TNsga2Individual">Concrete implementation of the users INsga2Individual</typeparam>
    public abstract class AbstractNsga2FitnessFunction<TNsga2Individual> : IFitnessFunction
        where TNsga2Individual : INsga2Individual
    {
        /// <summary>
        /// This method specify one way to test a given Individual of their fitness.
        /// It is being optimized towards negative infinity.
        /// </summary>
        /// <param name="individual">Individual in the type of the users implementation</param>
        /// <returns>the fitness of this individual for this fitness function</returns>
        protected abstract double DetermineFitness(TNsga2Individual individual);

        /// <summary>
        /// Implemented to simply cast the given IEvolutionaryAlgorithmIndividual to the given generic type.
        /// This is to prevent the user needing to cast every individual to their own INsga2Individual implementation,
        /// inside of their implemented fitness function.
        /// </summary>
        /// <param name="individual">Individual that is casted to the generic type</param>
        /// <returns>fitness value calculated by DetermineFitness(T individual)</returns>
        public double DetermineFitness(IEvolutionaryAlgorithmIndividual individual)
        {
            return DetermineFitness(individual is TNsga2Individual nsga2Individual ? nsga2Individual : default);
        }
    }
}