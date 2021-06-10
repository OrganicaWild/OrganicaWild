namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata
{
    public class TrialCell
    {
        public TrialCellState CurrentState { get; set; }
        public TrialCell[] Neighbors { get; set; }
        private TrialCellState NextState { get; set; }
        public TrialCellNetwork Network { get; set; }

        public TrialCell(TrialCellState state)
        {
            CurrentState = state;
        }

        public TrialCell()
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