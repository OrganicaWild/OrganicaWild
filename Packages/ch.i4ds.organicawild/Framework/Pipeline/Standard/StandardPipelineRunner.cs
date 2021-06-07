using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard
{
    public class StandardPipelineRunner : IPipelineRunner
    {
        public Random Random { get; }
        public int Seed { get; }
        
        private readonly IList<PipelineStep> executionPipeline;
        
        public StandardPipelineRunner(int? seed = null)
        {
            executionPipeline = new List<PipelineStep>();
            if (seed == null)
            {
                int tick = Environment.TickCount;
                Random = new Random(tick);
                Seed = tick;
            }
            else
            {
                Random = new Random(seed.Value);
                Seed = seed.Value;
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
        
        public IEnumerator Execute(Action<GameWorld> callback)
        {
            GameWorld world = null;
            
            foreach (PipelineStep step in executionPipeline)
            {
                yield return null;
                world = step.Apply(world);
            }

            callback(world);
        }

        public GameWorld ExecuteBlocking()
        {
            GameWorld world = null;
            
            foreach (PipelineStep step in executionPipeline)
            {
                world = step.Apply(world);
            }

            return world;
        }
    }
}