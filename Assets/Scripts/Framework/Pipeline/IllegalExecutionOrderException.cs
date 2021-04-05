using System;

namespace Framework.Pipeline
{
    public class IllegalExecutionOrderException : Exception
    {
        public IllegalExecutionOrderException() : base ($"Execution Order of PipelineSteps is not allowed")
        {
        }
    }
}