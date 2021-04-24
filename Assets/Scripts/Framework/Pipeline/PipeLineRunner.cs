using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Pipeline
{
    public class PipeLineRunner
    {
        private readonly IList<IPipelineStep> executionPipeline;
        private IThemeApplicator themeApplicator;
        private GameWorld world;

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
            
            foreach (IPipelineStep step in executionPipeline) world = step.Apply(world);

            return world;
        }

        public GameObject ApplyTheme()
        {
            return themeApplicator.Apply(world);
        }
    }
}