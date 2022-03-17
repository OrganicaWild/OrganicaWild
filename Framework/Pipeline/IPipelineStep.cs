using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline
{
    /// <summary>
    /// Defines the PipelineStep interface.
    /// This is an abstract class because it should inherit from MonoBehaviour, which is a class.
    /// </summary>
    public interface IPipelineStep
    {
        /// <summary>
        /// Random instance that is being instantiated by the PipelineRunner.
        /// Used to seed the whole generation process.
        /// </summary>
        public Random Rmg { get; set; }

        /// <summary>
        /// Defines what guarantees this Pipeline step expects from the previous Pipeline step
        /// </summary>
        public Type[] RequiredGuarantees { get; }

        /// <summary>
        /// defines, if this step will be included in the stacked debug view
        /// </summary>
        public virtual bool AddToDebugStackedView => false;
        
        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects { get; }

        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects { get; }
        
        /// <summary>
        /// Defines how the Pipeline step changes the GameWorld.
        /// </summary>
        /// <param name="world">GameWorld to change</param>
        /// <returns>changed GameWorld</returns>
        public GameWorld Apply(GameWorld world);
        
        public IPipelineStep[] ConnectedNextSteps { get; set; }
        public IPipelineStep[] ConnectedPreviousSteps { get; set; }

        public void Construct()
        {
            ConnectedNextSteps = new IPipelineStep[ProvidedOutputGameWorldObjects.Count];
            ConnectedPreviousSteps = new IPipelineStep[NeededInputGameWorldObjects.Count];
        }

        public IPipelineStep GetNextPipelineStep(int indexOfProvidedOutput)
        {
            if (indexOfProvidedOutput < 0 || indexOfProvidedOutput > ConnectedNextSteps.Length)
            {
                Debug.LogWarning($"Access index {indexOfProvidedOutput} when only index 0 to {ConnectedNextSteps.Length} exist.");
                return null;
            }

            return ConnectedNextSteps[indexOfProvidedOutput];
        }

        public void ConnectToNextPipelineStep(int indexOfProvidedOutput, IPipelineStep step)
        {
            ConnectedNextSteps[indexOfProvidedOutput] = step;
        }

        public void ConnectedToPreviousPipelineStep(int indexOfNeededInput, IPipelineStep step)
        {
            ConnectedPreviousSteps[indexOfNeededInput] = step;
        }

        public IPipelineStep GetPreviousPipelineStep(int indexOfNeededInput)
        {
            return ConnectedPreviousSteps[indexOfNeededInput];
        }

    }
}