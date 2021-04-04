using System;

namespace Framework.Pipeline
{
    public class NoChildPolicyException : Exception
    {
        public NoChildPolicyException() : base($"This IGameWorldObject is not allowed to have children.")
        {
        }
    }
}