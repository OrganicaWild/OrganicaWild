using System.Collections.Generic;

namespace Framework.Pipeline.GameWorldObjects
{
    public class AbstractLeafGameWorldObject : IGameWorldObject
    {
        public IGeometry Shape { get; set; }

        public IGameWorldObject this[int index]
        {
            get => throw new NoChildPolicyException();
            set => throw new NoChildPolicyException();
        }

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

        public int GetChildCount()
        {
            throw new NoChildPolicyException();
        }

        public void AddChildInChild(int childIndex, IGameWorldObject child)
        {
            throw new NoChildPolicyException();
        }
    }
}