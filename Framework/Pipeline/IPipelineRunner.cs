using System;
using System.Collections;

namespace Framework.Pipeline
{
    public interface IPipelineRunner
    {
        /// <summary>
        /// Adds a step to the pipeline.
        /// </summary>
        /// <param name="step">Pipeline Step to add</param>
        void AddStep(PipelineStep step);
        
        /// <summary>
        /// Execute all previously added Pipeline Steps.
        /// This method should be implemented as a Coroutine.
        /// </summary>
        /// <param name="callback">Action to work this the return value of coroutine</param>
        /// <returns>callback(gameWorld)</returns>
        IEnumerator Execute(Action<GameWorld> callback);
    }
}