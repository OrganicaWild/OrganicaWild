using System.Collections.Generic;
using Evolutionary.Nsga2;

namespace Evolutionary.Framework.Standard.Nsga2
{
    public class RankCrowdingComparer : Comparer<INsga2Individual>
    {
        public override int Compare(INsga2Individual x, INsga2Individual y)
        {
            if (y == null) return 1;
            if (x == null) return -1;
            
            if (x.Rank.CompareTo(y.Rank) != 0)
            {
                return x.Rank.CompareTo(y.Rank);
            }

            return x.Crowding.CompareTo(y.Crowding);
        }
    }
}