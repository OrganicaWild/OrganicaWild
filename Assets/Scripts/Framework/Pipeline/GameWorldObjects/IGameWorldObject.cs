using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public interface IGameWorldObject
    {
        IGeometry Shape { get; set; }

        IGameWorldObject this[int index] { get; set; }

        void AddChild(IGameWorldObject child);

        void RemoveChild(IGameWorldObject child);

        IEnumerable<IGameWorldObject> GetChildren();

        IEnumerable<IGameWorldObject> GetChildrenInChildren();

        IEnumerable<T> GetAllChildrenOfType<T>();

        void ClearChildren();

        int GetChildCount();

        /// <summary>
        /// Adds a IGameWorldObject to the child specified in the index.
        /// Implementations of the interface should implementing this function with a range check.
        /// </summary>
        /// <param name="childIndex">index of child</param>
        /// <param name="child">child to add</param>
        void AddChildInChild(int childIndex, IGameWorldObject child);

        IGameWorldObject GetParent();

        void SetParent(IGameWorldObject parent);
    }
}