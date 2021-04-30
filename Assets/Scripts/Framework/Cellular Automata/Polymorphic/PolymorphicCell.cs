using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;

public class PolymorphicCell
{
    public PolymorphicCellState CurrentState { get; set; }
    public PolymorphicCell[] Neighbors { get; set; }
    private PolymorphicCellState NextState { get; set; }
    public PolymorphicCellNetwork Network { get; set; }

    public PolymorphicCell(PolymorphicCellState state)
    {
        CurrentState = state;
    }

    public PolymorphicCell()
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