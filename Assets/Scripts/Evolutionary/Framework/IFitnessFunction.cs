namespace Evolutionary.Framework
{
    public interface IFitnessFunction<TGenoPhenoCombination>
        where TGenoPhenoCombination : IGenoPhenoCombination
    {
        double DetermineFitness(TGenoPhenoCombination phenoType);
    }
}