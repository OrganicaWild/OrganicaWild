namespace Evolutionary.Framework
{
    public interface IIndividual<TGenoPhenoCombination>
    {
        void EvaluatePhenoType();

        void AddFitnessFunction(IFitnessFunction<TGenoPhenoCombination> fitnessFunction);
    }
}