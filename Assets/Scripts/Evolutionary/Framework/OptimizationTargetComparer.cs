using System.Collections.Generic;

namespace Evolutionary.Framework
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
                return (int) (x.GetOptimizationTarget(optimizationTarget) -
                              y.GetOptimizationTarget(optimizationTarget));
            return 0;
        }
    }
}