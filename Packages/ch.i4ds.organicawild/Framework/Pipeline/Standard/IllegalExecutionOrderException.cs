using System;

namespace Framework.Pipeline.Standard
{
    public class IllegalExecutionOrderException : Exception
    {
        public IllegalExecutionOrderException() : base ($"Execution Order of PipelineSteps is not allowed")
        {
        }
    }
}