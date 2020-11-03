using Framework.Evolutionary.Nsga2;

namespace Framework.Evolutionary
{
    public interface IEvolutionaryAlgorithmIndividual
    {
        /// <summary>
        /// Evaluates the Fitness of one Individual.
        /// After this operation the GetOptimizationTarget(int index) call must return a value, when called with a valid index.
        /// </summary>
        void EvaluateFitness();

        int GetNumberOfFitnessFunctions();
        
        /// <summary>
        /// Return a certain fitness value of this Individual.
        /// For the Algorithm these are known as optimization targets.
        /// </summary>
        /// <param name="index">index of optimization target</param>
        /// <returns>fitness value of optimization target</returns>
        double GetOptimizationTarget(int index);

        /// <summary>
        /// Produce offspring of two IAlgorithmIndividuals. Implement this method according to what your Individuals look like.
        /// It is safe to cast parent2 back to the concrete Implementation, since all Individuals inside of an array must have the same Assigned type.
        /// </summary>
        /// <param name="parent2">second parent</param>
        /// <returns>New Instance of a child</returns>
        INsga2Individual MakeOffspring(INsga2Individual parent2);

        /// <summary>
        /// Prepare this individual for next generation.
        /// </summary>
        void PrepareForNextGeneration();
    }
}