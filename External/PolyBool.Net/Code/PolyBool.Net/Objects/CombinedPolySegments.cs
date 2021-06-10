using System.Collections.Generic;

namespace Polybool.Net.Objects
{
    public class CombinedPolySegments
    {
        public bool IsInverted1 { get; set; }

        public bool IsInverted2 { get; set; }

        public List<Segment> Combined { get; set; }
    }
}