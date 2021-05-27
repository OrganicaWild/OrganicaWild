namespace Framework.Cellular_Automata.Generic
{
    public class Cell<CellState>
    {
        public CellState CurrentState { get; set; }
        public Cell<CellState>[] Neighbors { get; set; }
        private CellState NextState { get; set; }
        public CellNetwork<CellState> Network { get; set; }

        public Cell(CellState state)
        {
            CurrentState = state;
        }

        public Cell()
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