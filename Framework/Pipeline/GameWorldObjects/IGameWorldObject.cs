using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public interface IGameWorldObject
    {
        /// <summary>
        /// String identifier for what type of IGameWorldObject this is.
        /// This is used in the ThemeApplicator to set a recipe for this Type.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// IGeometry Instance defining the geometric shape of this IGameWorldObject.
        /// </summary>
        IGeometry Shape { get; set; }

        IGameWorldObject this[int index] { get; set; }

        void AddChild(IGameWorldObject child);

        void RemoveChild(IGameWorldObject child);

        IEnumerable<IGameWorldObject> GetChildren();

        IEnumerable<IGameWorldObject> GetChildrenInChildren();

        IEnumerable<T> GetAllChildrenOfType<T>();

        bool HasAnyChildrenOfType<T>();

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