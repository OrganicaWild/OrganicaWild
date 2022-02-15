using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard
{
    /// <summary>
    /// Standard Implementation of PipelineRunner.
    /// Generally used inside of StandardPipelineManager but can also be used standalone.
    /// </summary>
    public class StandardPipelineRunner : IPipelineRunner
    {
        /// <summary>
        /// Random instance, that is passed to each step
        /// </summary>
        public Random Random { get; }
        
        /// <summary>
        /// Seed to instantiate random instance at the start of generation.
        /// </summary>
        public int Seed { get; }

        public static bool EncounteredError { get; private set; }

        private readonly IList<IPipelineStep> executionPipeline;

        public List<GameWorld> gameWorldInEachStep;

        public StandardPipelineRunner(int? seed = null)
        {
            executionPipeline = new List<IPipelineStep>();
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

        /// <summary>
        /// Add a step to the PipelineRunner.
        /// This method checks if the newly added step could be executed according to the required and supplied guarantees.
        /// </summary>
        /// <param name="step">step to add</param>
        /// <exception cref="IllegalExecutionOrderException">throws if previous step does not provide the required guarantees</exception>
        public void AddStep(IPipelineStep step)
        {
            IPipelineStep previous = executionPipeline.LastOrDefault();
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
                step.Rmg = Random;
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

            foreach (IPipelineStep step in executionPipeline)
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

            foreach (IPipelineStep step in executionPipeline)
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