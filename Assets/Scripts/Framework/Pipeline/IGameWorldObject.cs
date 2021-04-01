using System.Collections.Generic;

namespace Framework.Pipeline
{
    public interface IGameWorldObject
    {
        
        
        void AddChild(IGameWorldObject child);

        void RemoveChild(IGameWorldObject child);

        IEnumerable<IGameWorldObject> GetChildren();

        void ClearChildren();
        
        
    }
}