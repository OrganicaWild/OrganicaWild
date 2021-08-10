using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.Geometry;

namespace Framework.Pipeline.GameWorldObjects
{
    public abstract class AbstractGameWorldObject<TGeometry> : IGameWorldObject where TGeometry : IGeometry
    {
        private TGeometry shape;

        public string Identifier { get; set; }

        private IGameWorldObject parent;

        public AbstractGameWorldObject(TGeometry shape, string type = null)
        {
            this.shape = shape;
            Identifier = type;
            children = new List<IGameWorldObject>();
        }

        public TGeometry GetShape()
        {
            return shape;
        }

        IGeometry IGameWorldObject.GetShape()
        {
            return shape;
        }

        public void SetShape(IGeometry geometry)
        {
            if (geometry is TGeometry castGeometry)
            {
                shape = castGeometry;
            }
        }

        public IGameWorldObject this[int index]
        {
            get => children[index];
            set => children[index] = value;
        }

        private readonly IList<IGameWorldObject> children;


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

        public IEnumerable<T> GetAllChildrenOfType<T>() where T : IGameWorldObject
        {
            return children.Where(child => child is T).Select(x => x is T ? (T) x : default);
        }

        public IEnumerable<T> GetAllChildrenOfTypeRecursive<T>() where T : IGameWorldObject
        {
            List<T> children = GetAllChildrenOfType<T>().ToList();
            foreach (T child in children)
            {
                children.AddRange(child.GetAllChildrenOfTypeRecursive<T>().ToList());
            }

            return children;
        }

        public bool HasAnyChildrenOfType<T>()
        {
            return children.Any(child => child is T);
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

        public abstract IGameWorldObject Copy(Dictionary<int, object> identityDictionary);

        protected void CopyChildren(ref IGameWorldObject copy, Dictionary<int, object> identityDictionary)
        {
            //copy children
            foreach (IGameWorldObject child in GetChildren())
            {
                IGameWorldObject childCopy = child.Copy(identityDictionary);
                copy.AddChild(childCopy);
            }
        }
    }
}