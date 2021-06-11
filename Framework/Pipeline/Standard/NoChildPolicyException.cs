using System;

namespace Framework.Pipeline.Standard
{
    public class NoChildPolicyException : Exception
    {
        public NoChildPolicyException() : base($"This IGameWorldObject is not allowed to have children.")
        {
        }
    }
}