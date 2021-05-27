using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline
{
    public class PipeLineRunner
    {
        private readonly IList<PipelineStep> executionPipeline;
        private IThemeApplicator themeApplicator;
        public GameWorld World { get; private set; }

        public Random Random { get; }
        public int Seed { get; }

        public PipeLineRunner(int? seed = null)
        {
            executionPipeline = new List<PipelineStep>();
            if (seed == null)
            {
                int tick = Environment.TickCount;
                Random = new Random(tick);
                this.Seed = tick;
            }
            else
            {
                Random = new Random(seed.Value);
                this.Seed = seed.Value;
            }
        }

        public PipeLineRunner(IThemeApplicator themeApplicator)
        {
            if (themeApplicator != null)
            {
                this.themeApplicator = themeApplicator;
            }
        }

        public void AddStep(PipelineStep step)
        {
            PipelineStep previous = executionPipeline.LastOrDefault();
            Type[] requiredGuarantees = step.RequiredGuarantees;
            if (requiredGuarantees.All(requiredAttribute =>
                previous?.GetType().GetCustomAttributes().Any(attribute => attribute.GetType() == requiredAttribute) ??
                true))
            {
                executionPipeline.Add(step);
                step.random = Random;
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

        public IEnumerator Execute(Action<GameWorld> callback)
        {
            foreach (PipelineStep step in executionPipeline)
            {
                yield return null;
                World = step.Apply(World);
            }

            callback(World);
        }
    }
}