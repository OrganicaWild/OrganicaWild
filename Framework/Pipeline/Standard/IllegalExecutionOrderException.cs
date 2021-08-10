using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Pipeline.Standard
{
    /// <summary>
    /// Represents an error that can occur during creation of the Pipeline out of the supplied pipeline steps.
    /// </summary>
    public class IllegalExecutionOrderException : Exception
    {
        public readonly IEnumerable<Type> missingGuarantees;
        public readonly Type step;
        
        public IllegalExecutionOrderException(Type step, IEnumerable<Type> missingGuarantees) : base(
            $"{step} is missing the guarantees: \n {missingGuarantees.Select(x => x.Name).Aggregate(((type, type1) => type + ", " + type1))}")
        {
            this.step = step;
            this.missingGuarantees = missingGuarantees;
        }
    }
}