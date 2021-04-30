public interface IUpdateRule<CellState>
{
    CellState ApplyTo(Cell<CellState> cell);
}