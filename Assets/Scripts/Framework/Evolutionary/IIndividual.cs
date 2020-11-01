namespace Framework.Evolutionary
{
    public interface IIndividual<TGenoPhenoCombination> where TGenoPhenoCombination : IGenoPhenoCombination
    {
        TGenoPhenoCombination Representation { get; }
       void EvaluateFitnessFunctions();
    }
}