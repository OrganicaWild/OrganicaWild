using System.Collections.Generic;

namespace Evolutionary.Framework
{
    public interface INsga2Individual
    {
        void AddDominatedIndividual(INsga2Individual dominated);
        List<INsga2Individual> GetDominated();
        void IncrementDominationCount();
        void DecrementDominationCount();
        int GetDominationCount();
        double[] GetEvaluations();

        double GetOptimizationTarget(int index);
        void SetDistance(double distance);
        double GetDistance();
    }
}