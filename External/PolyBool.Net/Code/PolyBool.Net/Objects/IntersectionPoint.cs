namespace Polybool.Net.Objects
{
    public class IntersectionPoint
    {
        public Point Pt { get; set; }
        public int AlongA { get; set; }
        public int AlongB { get; set; }

        public IntersectionPoint(int alongA, int alongB, Point pt)
        {
            AlongA = alongA;
            AlongB = alongB;
            Pt = pt;
        }
    }
}