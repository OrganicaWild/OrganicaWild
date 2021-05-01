using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;

namespace Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata
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