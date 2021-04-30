using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public abstract class AbstractLeafGameWorldObject : IGameWorldObject
    {
        public string Type { get; set; }
        public IGeometry Shape { get; set; }
        
        private IGameWorldObject parent;
        
        public AbstractLeafGameWorldObject(IGeometry shape, string type = null)
        {
            this.Shape = shape;
            this.Type = type;
        }
        
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

        public IEnumerable<T> GetAllChildrenOfType<T>()
        {
            return Enumerable.Empty<T>();
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
    }
}