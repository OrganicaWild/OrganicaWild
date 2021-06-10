using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.GraphGrammar
{
    /// <summary>
    /// Provides an extension to the MissionVertex class with a Position, to make it drawable in the debug view.
    /// </summary>
    [Serializable]
    public class DrawableMissionVertex : MissionVertex
    {
        private float x;
        private float z;
        
        /// <summary>
        /// Create new DrawableMissionVertex with the supplied Type
        /// </summary>
        /// <param name="type">mission part</param>
        public DrawableMissionVertex(string type) : base(type)
        {
            x = Random.value;
            z = Random.value * 2 - 1;
        }

        internal List<Branch> Paint(Vector3 parentPosition,
            List<Branch> drawPositions, Dictionary<MissionVertex, Vector3> dictionary)
        {
            Vector3 thisPosition;
            
            if (dictionary.ContainsKey(this))
            {
                thisPosition = dictionary[this];
            }
            else
            {
                thisPosition = parentPosition + new Vector3(x, 0, z).normalized +
                               new Vector3(x, 0, 0);
                dictionary.Add(this, thisPosition);
               
            }
            drawPositions.Add(new Branch(this, parentPosition, thisPosition));
            // Debug.DrawLine(parentPosition, thisPosition, Color.blue, 100000);

            foreach (MissionVertex forwardNeighbour in ForwardNeighbours)
            {
                if (forwardNeighbour is DrawableMissionVertex draw)
                {
                    draw.Paint(thisPosition, drawPositions, dictionary);
                }
            }

            return drawPositions;
        }

        internal class Branch
        {
            public readonly DrawableMissionVertex t;
            public Vector3 parent;
            public Vector3 tPosition;

            public Branch(DrawableMissionVertex t, Vector3 parent, Vector3 tPosition)
            {
                this.t = t;
                this.parent = parent;
                this.tPosition = tPosition;
            }
        }
    }
}