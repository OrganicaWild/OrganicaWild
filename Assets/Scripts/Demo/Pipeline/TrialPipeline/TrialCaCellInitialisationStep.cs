using Assets.Scripts.Demo.Pipeline.TrialPipeline;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;

namespace Assets.Scripts.Demo.Pipeline.TrialPipeLine
{
    public class TrialCaCellInitialisationStep : CaCellInitialisationStep
    {
        public override TrialCellState InitialLandmarkAreaCellState { get; set; }
    }
}