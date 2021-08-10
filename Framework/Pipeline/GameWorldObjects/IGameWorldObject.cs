using System.Collections.Generic;
using Framework.Pipeline.Geometry;
using Framework.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    /// <summary>
    /// Interface defining a single node in the pipeline data structure.
    /// Implementations of this interface define specific things in the GameWorld like Area, Landmark, or even more specific, like SpaceStation.
    /// Implementations may directly implement this Interface or build on top of the partially implemented AbstractGameWorldObject.
    /// </summary>
    public interface IGameWorldObject
    {
        /// <summary>
        /// String identifier for what type of IGameWorldObject this is.
        /// </summary>
        string Identifier { get; set; }
        
        /// <summary>
        /// Get the geometric shape that represents this IGameWorldObject.
        /// </summary>
        IGeometry GetShape();
        
        /// <summary>
        /// Set the geometric shape that represents this IGameWorldObject.
        /// </summary>
        /// <param name="geometry">IGeometry to represent this IGameWorldObject</param>
        void SetShape(IGeometry geometry);

        /// <summary>
        /// Get Children of this IGameWorldObject at specified index.
        /// </summary>
        /// <param name="index"></param>
        IGameWorldObject this[int index] { get; set; }

        /// <summary>
        /// Add a new child to this IGameWorldObject.
        /// If supplied child is null it is not added as a child.
        /// </summary>
        /// <param name="child">IGameWorldObject to add as a child</param>
        void AddChild(IGameWorldObject child);

        /// <summary>
        /// Remove a child.
        /// If supplied child is not a child of this IGameWorldObject the program continues silently.
        /// If supplied child is null the program continues silently
        /// </summary>
        /// <param name="child">IGameWorldObject to remove as child</param>
        void RemoveChild(IGameWorldObject child);

        /// <summary>
        /// Get all children of this IGameWorldObject
        /// </summary>
        /// <returns>All children</returns>
        IEnumerable<IGameWorldObject> GetChildren();

        /// <summary>
        /// Get all children in children reduced to a single list.
        /// </summary>
        /// <returns>list containing all children and grandchildren</returns>
        IEnumerable<IGameWorldObject> GetChildrenInChildren();

        /// <summary>
        /// Get all children of specified type T.
        /// </summary>
        /// <typeparam name="T">IGameWorldType to search for in children</typeparam>
        /// <returns>list containing all children of type T</returns>
        IEnumerable<T> GetAllChildrenOfType<T>() where T : IGameWorldObject;


        IEnumerable<T> GetAllChildrenOfTypeRecursive<T>() where T : IGameWorldObject;

        /// <summary>
        /// Check whether this IGameWorldObject has any children of the specified type T
        /// </summary>
        /// <typeparam name="T">IGameWorldType to test for in children</typeparam>
        /// <returns>true if there are any children of Type T</returns>
        bool HasAnyChildrenOfType<T>();

        /// <summary>
        /// Remove all children from this IGameWorldObject.
        /// This method clears the underlying data structure.
        /// </summary>
        void ClearChildren();

        /// <summary>
        /// Get number of children
        /// </summary>
        /// <returns>number of children</returns>
        int GetChildCount();

        /// <summary>
        /// Adds a IGameWorldObject to the child specified in the index.
        /// Implementations of the interface should implementing this function with a range check.
        /// </summary>
        /// <param name="childIndex">index of child</param>
        /// <param name="child">child to add</param>
        void AddChildInChild(int childIndex, IGameWorldObject child);

        /// <summary>
        /// Get parent of this IGameWorldObject.
        /// </summary>
        /// <returns>parent of IGameWorldObject if present, or </returns>
        IGameWorldObject GetParent();

        /// <summary>
        /// Set parent of this IGameWorldObject.
        /// </summary>
        /// <param name="parent">IGameWorldObject to become parent of this object</param>
        void SetParent(IGameWorldObject parent);

        /// <summary>
        /// Deep Copy of IGameWorldObject and all children.
        /// </summary>
        /// <param name="identityDictionary">Identity Dictionary to prevent double copying in case of multiple inheritance</param>
        /// <returns>deep copy of this</returns>
        IGameWorldObject Copy(Dictionary<int, object> identityDictionary);
    }
}