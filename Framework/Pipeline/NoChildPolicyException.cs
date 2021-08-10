using System;

namespace Framework.Pipeline
{
    /// <summary>
    /// Represents an error that may occur if it is tried to add a child to a AbstractLeafGameWorldObject.
    /// </summary>
    public class NoChildPolicyException : Exception
    {
        public NoChildPolicyException() : base($"This IGameWorldObject is not allowed to have children.")
        {
        }
    }
}