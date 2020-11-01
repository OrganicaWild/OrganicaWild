using System.Collections.Generic;

namespace Evolutionary.Framework.Nsga2
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
        double GetOptimizationTarget(int index);
        INsga2Individual MakeOffspring(INsga2Individual parent2);
        void CleanUp();
    }
}