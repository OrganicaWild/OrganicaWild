using System.Collections.Generic;
using System.Linq;

namespace Framework.Pipeline.GameWorldObjects
{
    public class AbstractGameWorldObject : IGameWorldObject
    {
        public IGeometry Shape { get; set; }

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
            }
        }

        public void RemoveChild(IGameWorldObject child)
        {
            if (child != null)
            {
                children.Remove(child);
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
            }
        }
    }
}