using System.Collections.Generic;

namespace Framework.Evolutionary.Nsga2
{
    /// <summary>
    /// Comparer to compare two INsga2Individuals based on Rank first and secondly on crowding distance
    /// </summary>
    public class RankCrowdingComparer : Comparer<INsga2Individual>
    {
        /// <summary>
        /// Sort two INsga2Individuals firstly on rank and secondly on crowding distance.
        /// </summary>
        /// <param name="x">first individual</param>
        /// <param name="y">second individual</param>
        /// <returns>
        ///     negative value if y is larger than x
        ///     0 if y and x are the same
        ///     positive value if x is larger than y
        /// </returns>
        public override int Compare(INsga2Individual x, INsga2Individual y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }
            
            if (x.Rank.CompareTo(y.Rank) != 0)
            {
                return x.Rank.CompareTo(y.Rank);
            }

            return x.Crowding.CompareTo(y.Crowding);
        }
    }
}