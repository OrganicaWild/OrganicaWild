namespace Assets.Scripts.Framework.Cellular
{
    public abstract class CACell
    {
        public int Index { get; set; }
        public CANetwork Network { get; set; }

        public CACell(int index)
        {
            Index = index;
        }
        public abstract CACell Update();
    }
}
