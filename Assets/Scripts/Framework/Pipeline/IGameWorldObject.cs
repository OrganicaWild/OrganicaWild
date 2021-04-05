using System.Collections.Generic;

namespace Framework.Pipeline
{
    public interface IGameWorldObject
    {
        public IGeometry Shape { get; set; }

        public IGameWorldObject this[int index]
        {
            get;
            set;
        }

        void AddChild(IGameWorldObject child);

        void RemoveChild(IGameWorldObject child);

        IEnumerable<IGameWorldObject> GetChildren();

        IEnumerable<IGameWorldObject> GetChildrenInChildren();

        void ClearChildren();

        int GetChildCount();
        
        /// <summary>
        /// Adds a IGameWorldObject to the child specified in the index.
        /// Implementations of the interface should implementing this function with a range check.
        /// </summary>
        /// <param name="childIndex">index of child</param>
        /// <param name="child">child to add</param>
        void AddChildInChild(int childIndex, IGameWorldObject child);
    }
}