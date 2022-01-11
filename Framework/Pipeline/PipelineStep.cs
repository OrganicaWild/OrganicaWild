using System;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline
{
    /// <summary>
    /// Defines the PipelineStep interface.
    /// This is an abstract class because it should inherit from MonoBehaviour, which is a class.
    /// </summary>
    public abstract class PipelineStep
    {

        public PipelineStep()
        {
        }

        /// <summary>
        /// Random instance that is being instantiated by the PipelineRunner.
        /// Used to seed the whole generation process.
        /// </summary>
        public Random random;
        
        /// <summary>
        /// Defines what guarantees this Pipeline step expects from the previous Pipeline step
        /// </summary>
        public abstract Type[] RequiredGuarantees { get; }
        
        /// <summary>
        /// defines, if this step will be included in the stacked debug view
        /// </summary>
        public virtual bool AddToDebugStackedView => false;

        /// <summary>
        /// Defines how the Pipeline step changes the GameWorld.
        /// </summary>
        /// <param name="world">GameWorld to change</param>
        /// <returns>changed GameWorld</returns>
        public abstract GameWorld Apply(GameWorld world);
        
    }
}