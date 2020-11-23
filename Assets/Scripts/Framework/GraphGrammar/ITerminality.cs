using System;

namespace Framework.GraphGrammar
{
    public interface ITerminality
    {
        bool IsTerminal();
        bool Equals(object other);
    }
}