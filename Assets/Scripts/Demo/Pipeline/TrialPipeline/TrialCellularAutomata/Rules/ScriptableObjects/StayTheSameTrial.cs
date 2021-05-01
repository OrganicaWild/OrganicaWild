using Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;

public class StayTheSameTrial : TrialUpdateRule
{
    public override TrialCellState ApplyTo(TrialCell cell)
    {
        return cell.CurrentState;
    }
}
