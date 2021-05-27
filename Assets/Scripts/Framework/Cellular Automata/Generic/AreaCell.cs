using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;

namespace Framework.Cellular_Automata.Generic
{
    public class AreaCell<CellState> : Area
    {
        public Cell<CellState> Cell { get; set; }

        public AreaCell(IGeometry shape, Cell<CellState> cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}