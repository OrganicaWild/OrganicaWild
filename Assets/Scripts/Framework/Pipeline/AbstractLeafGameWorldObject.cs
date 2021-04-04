using System.Collections.Generic;

namespace Framework.Pipeline
{
    public abstract class AbstractLeafGameWorldObject : IGameWorldObject
    {
        public IGeometry Shape { get; set; }

        public void AddChild(IGameWorldObject child)
        {
            throw new NoChildPolicyException();
        }

        public void RemoveChild(IGameWorldObject child)
        {
            throw new NoChildPolicyException();
        }

        public IEnumerable<IGameWorldObject> GetChildren()
        {
            throw new NoChildPolicyException();
        }

        public IEnumerable<IGameWorldObject> GetChildrenInChildren()
        {
            throw new NoChildPolicyException();
        }

        public void ClearChildren()
        {
            throw new NoChildPolicyException();
        }
    }
}