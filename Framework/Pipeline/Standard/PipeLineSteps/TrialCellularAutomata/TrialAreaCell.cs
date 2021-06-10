using Framework.Pipeline.GameWorldObjects;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata
{
    public class TrialAreaCell : Area
    {
        public TrialCell Cell { get; set; }

        public TrialAreaCell(IGeometry shape, TrialCell cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}