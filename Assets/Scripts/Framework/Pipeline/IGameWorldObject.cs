using System.Collections.Generic;

namespace Framework.Pipeline
{
    public interface IGameWorldObject
    {
        public IGeometry Shape { get; set; }
        
        void AddChild(IGameWorldObject child);

        void RemoveChild(IGameWorldObject child);

        IEnumerable<IGameWorldObject> GetChildren();

        IEnumerable<IGameWorldObject> GetChildrenInChildren();

        void ClearChildren();
        
        
    }
}