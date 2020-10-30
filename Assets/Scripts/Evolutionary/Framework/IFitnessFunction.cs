namespace Evolutionary.Framework
{
    public interface IFitnessFunction<TPhenoType>
    {
        double Apply(TPhenoType phenoType);
    }
}