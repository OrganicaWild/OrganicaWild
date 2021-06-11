namespace Framework.Cellular_Automata.Byte
{
    public class ByteCell
    {
        public byte CurrentState { get; set; }
        public ByteCell[] Neighbors { get; set; }
        private byte NextState { get; set; }
        public ByteCellNetwork Network { get; set; }

        public ByteCell(byte state)
        {
            CurrentState = state;
        }

        public ByteCell()
        {
        }

        public void CalculateUpdate()
        {
            NextState = Network.UpdateRules[CurrentState].ApplyTo(this);
        }

        public void ExecuteUpdate()
        {
            CurrentState = NextState;
        }
    }
}