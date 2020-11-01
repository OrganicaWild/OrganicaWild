namespace Evolutionary.Framework
{
    public interface IIndividual<TGenoPhenoCombination> where TGenoPhenoCombination : IGenoPhenoCombination
    {
        TGenoPhenoCombination Representation { get; }
       void EvaluateFitnessFunctions();
    }
}