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

        public static bool EncounteredError { get; private set; }

        private readonly IList<PipelineStep> executionPipeline;

        public List<GameWorld> gameWorldInEachStep;

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
                Seed = (int) seed;
            }
        }

        public void AddStep(PipelineStep step)
        {
            PipelineStep previous = executionPipeline.LastOrDefault();
            Type[] requiredGuarantees = step.RequiredGuarantees;
            List<Attribute> providedGuarantees = previous?.GetType().GetCustomAttributes().ToList();
            List<Type> notMetRequiredGuarantees = new List<Type>();

            foreach (Type requiredGuarantee in requiredGuarantees)
            {
                var found = false;
                if (providedGuarantees != null)
                    foreach (Attribute providedGuarantee in providedGuarantees)
                    {
                        //if required guarantee can be found in provided guarantee, remove it from required list
                        if (requiredGuarantee == providedGuarantee.GetType())
                        {
                            found = true;
                            break;
                        }
                    }

                if (!found)
                {
                    notMetRequiredGuarantees.Add(requiredGuarantee);
                }
            }

            if (!notMetRequiredGuarantees.Any())
            {
                executionPipeline.Add(step);
                step.random = Random;
            }
            else
            {
                throw new IllegalExecutionOrderException(step.GetType(), notMetRequiredGuarantees);
            }
            
        }

        public IEnumerator Execute(Action<GameWorld> callback)
        {
            GameWorld world = null;
            gameWorldInEachStep = new List<GameWorld>();

            foreach (PipelineStep step in executionPipeline)
            {
                yield return null;
                try
                {
                    world = step.Apply(world);
                    gameWorldInEachStep.Add(world.Copy());
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    EncounteredError = true;
                    yield break;
                }
            }

            callback(world);
            EncounteredError = false;
        }

        public GameWorld ExecuteBlocking()
        {
            GameWorld world = null;
            gameWorldInEachStep = new List<GameWorld>();

            foreach (PipelineStep step in executionPipeline)
            {
                world = step.Apply(world);
                if (step.AddToDebugStackedView)
                {
                    gameWorldInEachStep.Add(world.Copy());
                }
            }

            return world;
        }
    }
}