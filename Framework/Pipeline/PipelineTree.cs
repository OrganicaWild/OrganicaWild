using System.Collections.Generic;
using Framework.Pipeline.Standard.PipeLineSteps.SmallSteps;

namespace Framework.Pipeline
{
    public class PipelineTree
    {
        private OriginStep originStep;

        private List<IPipelineStep> steps;

        public PipelineTree()
        {
            this.originStep = new OriginStep();
            this.steps = new List<IPipelineStep>();
        }
    }
}