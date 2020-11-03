using System.Collections.Generic;

namespace Framework.Evolutionary.Nsga2
{
    /// <summary>
    /// Comparer to compare INsga2Individuals based on a given optimization target.
    /// </summary>
    public class OptimizationTargetComparer : Comparer<INsga2Individual>
    {
        /// <summary>
        /// optimization target to sort against.
        /// </summary>
        private readonly int optimizationTarget;

        public OptimizationTargetComparer(int optimizationTarget)
        {
            this.optimizationTarget = optimizationTarget;
        }

        /// <summary>
        /// Compare two INsga2Individuals based on optimization target
        /// </summary>
        /// <param name="x">First individual</param>
        /// <param name="y">Second individual</param>
        /// <returns>
        ///     negative value if y is larger than x
        ///     0 if y and x are the same
        ///     positive value if x is larger than y
        /// </returns>
        public override int Compare(INsga2Individual x, INsga2Individual y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return (x.GetOptimizationTarget(optimizationTarget).CompareTo(
                y.GetOptimizationTarget(optimizationTarget)));
        }
    }
}