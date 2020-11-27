using System;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    [Serializable]
    public class MeshCorner : IComparable<MeshCorner>
    {
        internal Vector3 center;

        public Vector3 connectionPoint;
        public Vector3 connectionDirection;


        public int CompareTo(MeshCorner other)
        {
            return (int) (
                (other.connectionPoint.x - center.x) * (connectionPoint.z - center.z) -
                (connectionPoint.x - center.x) * (other.connectionPoint.z - center.z));
        }
        
    }
}