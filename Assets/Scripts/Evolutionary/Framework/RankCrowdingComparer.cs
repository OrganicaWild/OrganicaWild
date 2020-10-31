using System;
using System.Collections.Generic;

namespace Evolutionary.Framework
{
    public class RankCrowdingComparer : Comparer<INsga2Individual>
    {
        public override int Compare(INsga2Individual x, INsga2Individual y)
        {
            if (x.Rank.CompareTo(y.Rank) != 0)
            {
                return x.Rank.CompareTo(y.Rank);
            }

            return x.Crowding.CompareTo(y.Crowding);
        }
    }
}