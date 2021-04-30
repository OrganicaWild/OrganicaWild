using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.ThemeApplicator.Recipe;

public class AreaCell<CellState> : Area
{
    public Cell<CellState> Cell { get; set; }

    public AreaCell(IGeometry shape, Cell<CellState> cell, string type = null) : base(shape, type)
    {
        Cell = cell;
    }
}