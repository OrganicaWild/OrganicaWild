namespace Framework.Util
{
    public class MutableTuple<TType>
    {
        public TType Item1 { get; set; }
        public TType Item2 { get; set; }

        public MutableTuple(TType item1, TType item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}