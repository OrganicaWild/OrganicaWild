namespace Framework.Cellular_Automata.Generic
{
    public interface IUpdateRule<CellState>
    {
        CellState ApplyTo(Cell<CellState> cell);
    }
}