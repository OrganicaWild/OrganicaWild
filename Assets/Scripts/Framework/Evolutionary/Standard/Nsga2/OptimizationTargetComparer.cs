using System.Collections.Generic;

namespace Framework.Evolutionary.Standard.Nsga2
{
    public class OptimizationTargetComparer : Comparer<INsga2Individual>
    {
        private int optimizationTarget;

        public OptimizationTargetComparer(int optimizationTarget)
        {
            this.optimizationTarget = optimizationTarget;
        }

        public override int Compare(INsga2Individual x, INsga2Individual y)
        {
            if (x != null && y != null)
                return (x.GetOptimizationTarget(optimizationTarget).CompareTo(
                              y.GetOptimizationTarget(optimizationTarget)));
            return 0;
        }
    }
}