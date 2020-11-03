using System;

namespace Framework.Evolutionary.Nsga2
{
    public class FitnessFunctionNumberMismatchException : Exception
    {
        public FitnessFunctionNumberMismatchException(string message) : base(message)
        {
        }
    }
}