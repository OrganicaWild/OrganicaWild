using System.Collections.Generic;
using System.Linq;

namespace Polybool.Net.Objects
{
    public class Polygon
    {
        public Polygon()
        {
            Regions = new List<Region>();
        }

        public Polygon(List<Region> regions, bool isInverted = false)
        {
            Regions = regions;
            Inverted = isInverted;
        }

        public bool IsEmpty()
        {
            int sum = Regions.Sum(region => region.Points.Count);
            return sum == 0;
        }

        public bool Equals(Polygon other)
        {
            if (other.Regions.Count != Regions.Count)
            {
                return false;
            }
            
            int equalAreas = 0;
            foreach (Region region in Regions)
            {
                foreach (Region otherRegion in other.Regions)
                {
                    if (otherRegion.Equals(region))
                    {
                        //found region in other polygon
                        equalAreas++;
                        break;
                    }
                }
            }

            return equalAreas == Regions.Count;
        }

        public List<Region> Regions { get; set; }
        public bool Inverted { get; set; }
    }
}