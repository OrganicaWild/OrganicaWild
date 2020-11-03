namespace Framework.Evolutionary.Nsga2
{
    public abstract class AbstractNsga2FitnessFunction<TNsga2Individual> : IFitnessFunction where TNsga2Individual : INsga2Individual
    {
        protected abstract double DetermineFitness(TNsga2Individual individual);

        public double DetermineFitness(IAlgorithmIndividual individual)
        {
            return DetermineFitness(individual is TNsga2Individual ? (TNsga2Individual) individual : default);
        }
    }
}