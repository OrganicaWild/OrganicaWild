using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Standard;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
public abstract class AbstractLeafGameWorldObject<TGeometry> : IGameWorldObject where TGeometry : IGeometry
    {
        public string Identifier { get; set; }

        private TGeometry shape;

        private IGameWorldObject parent;
        
        public AbstractLeafGameWorldObject(TGeometry shape, string type = null)
        {
            this.shape = shape;
            this.Identifier = type;
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

        public IEnumerable<T> GetAllChildrenOfType<T>() where T : IGameWorldObject
        {
            return Enumerable.Empty<T>();
        }

        public IEnumerable<T> GetAllChildrenOfTypeRecursive<T>() where T : IGameWorldObject
        {
            return Enumerable.Empty<T>();
        }

        public bool HasAnyChildrenOfType<T>()
        {
            return false;
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

        public abstract IGameWorldObject Copy(Dictionary<int, object> identityDictionary);
    }
}