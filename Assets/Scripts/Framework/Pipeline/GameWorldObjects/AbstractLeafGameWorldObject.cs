using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public class AbstractLeafGameWorldObject : IGameWorldObject
    {
        public IGeometry Shape { get; set; }
        private IGameWorldObject parent;

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
            return Enumerable.Empty<IGameWorldObject>();
        }

        public IEnumerable<IGameWorldObject> GetChildrenInChildren()
        {
            return Enumerable.Empty<IGameWorldObject>();
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

        public IGameWorldObject GetParent()
        {
            return parent;
        }

        public void SetParent(IGameWorldObject parent)
        {
            this.parent = parent;
        }

        public Vector2 GetLocalPosition()
        {
            return Shape.GetCentroid();
        }

        public Vector2 GetGlobalPosition()
        {
            if (parent == null)
            {
                return GetLocalPosition();
            }
            return parent.GetGlobalPosition() + GetLocalPosition();
        }
    }
}