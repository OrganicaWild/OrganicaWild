using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata
{
    public class TrialAreaCell : Area
    {
        public TrialCell Cell { get; set; }

        public TrialAreaCell(OwPolygon shape, TrialCell cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}