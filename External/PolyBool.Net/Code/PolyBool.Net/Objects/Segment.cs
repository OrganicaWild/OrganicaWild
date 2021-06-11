namespace Polybool.Net.Objects
{
    public class Segment
    {
        public Point End { get; set; }
        public Point Start { get; set; }
        public Fill MyFill { get; set; }
        public Fill OtherFill { get; set; }
    }
}