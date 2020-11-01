namespace Framework.Evolutionary
{
    public interface IFitnessFunction<TGenoPhenoCombination>
        where TGenoPhenoCombination : IGenoPhenoCombination
    {
        double DetermineFitness(TGenoPhenoCombination phenoType);
    }
}