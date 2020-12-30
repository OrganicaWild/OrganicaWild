using System;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    [Serializable]
    public class SpaceNodeConnection : IComparable<SpaceNodeConnection>
    {
        internal Vector3 center;

        /// <summary>
        /// Point in 3D space where the connection ends.
        /// </summary>
        public Vector3 connectionPoint;
        
        /// <summary>
        /// The direction in which the new Space Nodes are attached to.
        /// </summary>
        public Vector3 connectionDirection;
        
        public int CompareTo(SpaceNodeConnection other)
        {
            return (int) (
                (other.connectionPoint.x - center.x) * (connectionPoint.z - center.z) -
                (connectionPoint.x - center.x) * (other.connectionPoint.z - center.z));
        }
    }
}