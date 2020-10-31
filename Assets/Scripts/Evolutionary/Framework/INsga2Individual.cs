using System.Collections.Generic;

namespace Evolutionary.Framework
{
    public interface INsga2Individual
    {
        int Rank { get; set; }
        double Crowding { get; set; }
        void AddDominatedIndividual(INsga2Individual dominated);
        List<INsga2Individual> GetDominated();
        void IncrementDominationCount();
        void DecrementDominationCount();
        int GetDominationCount();
        double[] GetEvaluations();
        double GetOptimizationTarget(int index);
        void SetDistance(double distance);
        double GetDistance();
        INsga2Individual MakeOffspring(INsga2Individual parent2);
    }
}