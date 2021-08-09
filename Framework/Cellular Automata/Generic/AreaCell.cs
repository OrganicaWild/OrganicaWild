using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;

namespace Framework.Cellular_Automata.Generic
{
    public class AreaCell<CellState> : Area
    {
        public Cell<CellState> Cell { get; set; }

        public AreaCell(OwPolygon shape, Cell<CellState> cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}