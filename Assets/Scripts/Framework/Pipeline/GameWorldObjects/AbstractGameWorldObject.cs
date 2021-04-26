using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public class AbstractGameWorldObject : IGameWorldObject
    {
        public IGeometry Shape { get; set; }
        private IGameWorldObject parent;

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