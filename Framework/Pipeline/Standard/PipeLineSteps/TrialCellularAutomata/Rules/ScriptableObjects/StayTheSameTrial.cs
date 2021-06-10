namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects
{
    public class StayTheSameTrial : TrialUpdateRule
    {
        public override TrialCellState ApplyTo(TrialCell cell)
        {
            return cell.CurrentState;
        }
    }
}
