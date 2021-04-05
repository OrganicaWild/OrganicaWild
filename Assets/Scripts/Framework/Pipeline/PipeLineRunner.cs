using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Pipeline
{
    public class PipeLineRunner
    {
        private IList<IPipelineStep> executionPipeline;
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
            IPipelineStep last = executionPipeline.Last();
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

        public GameObject Execute()
        {
            GameWorld world = null;
            foreach (IPipelineStep step in executionPipeline)
            {
                world = step.Apply(world);
            }

            return themeApplicator.Apply(world);
        }
    }
}