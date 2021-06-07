using System;

namespace Framework.Evolutionary.Nsga2
{
    /// <summary>
    /// Exception indicating that the fitness functions inside of an algorithm are not the same on the different Individuals.
    /// </summary>
    public class FitnessFunctionNumberMismatchException : Exception
    {
        public FitnessFunctionNumberMismatchException(string message) : base(message)
        {
        }
    }
}