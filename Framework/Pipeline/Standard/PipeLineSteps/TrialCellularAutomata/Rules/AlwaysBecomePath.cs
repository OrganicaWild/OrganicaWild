namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects
{
    public class AlwaysBecomePath : TrialUpdateRule
    {
        public override TrialCellState ApplyTo(TrialCell cell)
        {
            return TrialCellState.Path;
        }
    }
}
