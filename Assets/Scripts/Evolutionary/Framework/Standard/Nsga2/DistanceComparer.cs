using System.Collections.Generic;

namespace Evolutionary.Framework.Standard.Nsga2
{
   /// <summary>
   /// Comparer for INsga2Individuals to compare based on Crowding Distance.
   /// Used inside of the Nsga-2 Algorithm.
   /// </summary>
    public class DistanceComparer : Comparer<INsga2Individual>
    {
       /// <summary>
       /// Compare two INsga2Individuals based on Crowding Distance
       /// </summary>
       /// <param name="x">First Individual</param>
       /// <param name="y">Second Indivdual</param>
       /// <returns>
       ///     negative value if y is larger than x
       ///     0 if y and x are the same
       ///     positive value if x is larger than y
       /// </returns>
        public override int Compare(INsga2Individual x, INsga2Individual y)
        {
            if (x != null && y != null)
                return (x.Crowding.CompareTo(y.Crowding));
            return 0;
        }
    }
}