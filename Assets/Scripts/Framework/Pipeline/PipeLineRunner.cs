using System.Collections.Generic;
using System.Linq;

namespace Framework.Pipeline
{
    public class PipeLineRunner
    {
        private readonly IList<IPipelineStep> executionPipeline;
        private IThemeApplicator themeApplicator;

        public PipeLineRunner()
        {
            executionPipeline = new List<IPipelineStep>();
        }

        public PipeLineRunner(IThemeApplicator themeApplicator)
        {
            if (themeApplicator != null)
            {
                this.themeApplicator = themeApplicator;
            }
        }

        public void AddStep(IPipelineStep step)
        {
            IPipelineStep last = executionPipeline.LastOrDefault();
            if (step.IsValidStep(last))
            {
                executionPipeline.Add(step);
            }
            else
            {
                throw new IllegalExecutionOrderException();
            }
        }

        public void SetThemeApplicator(IThemeApplicator themeApplicator)
        {
            if (themeApplicator != null)
            {
                this.themeApplicator = themeApplicator;
            }
        }

        public GameWorld Execute()
        {
            GameWorld world = null;
            foreach (IPipelineStep step in executionPipeline) world = step.Apply(world);

            return world;
        }
    }
}