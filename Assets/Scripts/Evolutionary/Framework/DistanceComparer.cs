using System.Collections.Generic;

namespace Evolutionary.Framework
{
    public class DistanceComparer : Comparer<INsga2Individual>
    {
        public override int Compare(INsga2Individual x, INsga2Individual y)
        {
            if (x != null && y != null)
                return (int) (x.Crowding.CompareTo(y.Crowding));
            return 0;
        }
    }
}