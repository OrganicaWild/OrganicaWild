using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.ThemeApplicator.Recipe;

namespace Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata
{
    public class TrialAreaCell : Area
    {
        public TrialCell Cell { get; set; }

        public TrialAreaCell(IGeometry shape, TrialCell cell, GameWorldObjectRecipe recipe = null) : base(shape, recipe)
        {
            Cell = cell;
        }
    }
}