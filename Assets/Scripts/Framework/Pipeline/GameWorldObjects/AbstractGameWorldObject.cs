using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public abstract class AbstractGameWorldObject : IGameWorldObject
    {
        public IGeometry Shape { get; set; }
        private IGameWorldObject parent;
        public IGameWorldObjectRecipe recipe;
        
        public IGameWorldObjectRecipe GetRecipe()
        {
            return recipe;
        }

        public IGameWorldObject this[int index]
        {
            get => children[index];
            set => children[index] = value;
        }

        private readonly IList<IGameWorldObject> children;

        public AbstractGameWorldObject()
        {
            children = new List<IGameWorldObject>();
        }

        public void AddChild(IGameWorldObject child)
        {
            if (child != null && !children.Contains(child))
            {
                children.Add(child);
                child.SetParent(this);
            }
        }

        public void RemoveChild(IGameWorldObject child)
        {
            if (child != null)
            {
                children.Remove(child);
                child.SetParent(null);
            }
        }

        public IEnumerable<IGameWorldObject> GetChildren()
        {
            return children;
        }

        public IEnumerable<IGameWorldObject> GetChildrenInChildren()
        {
            return children.SelectMany(child => child.GetChildren());
        }

        public IEnumerable<T> GetAllChildrenOfType<T>()
        {
            return children.Where(child => child is T).Select(x => x is T ? (T) x : default);
        }

        public void ClearChildren()
        {
            children.Clear();
        }

        public int GetChildCount()
        {
            return children.Count;
        }

        public void AddChildInChild(int childIndex, IGameWorldObject child)
        {
            if (!(childIndex < 0 && childIndex > GetChildCount() - 1))
            {
                children[childIndex].AddChild(child);
                child.SetParent(children[childIndex]);
            }
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