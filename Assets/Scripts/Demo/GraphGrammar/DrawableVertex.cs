using System;
using System.Collections.Generic;
using Framework.GraphGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.GraphGrammar
{
    [Serializable]
    public class DrawableVertex<TType> : Vertex<TType>
    {
        private float x;
        private float z;
        
        public DrawableVertex(TType type) : base(type)
        {
            x = Random.value;
            z = (Random.value * 2) - 1;
        }

        public List<ListElement> Paint(Vector3 parentPosition,
            List<ListElement> drawPositions, Dictionary<Vertex<TType>, Vector3> dictionary)
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
            drawPositions.Add(new ListElement(this, parentPosition, thisPosition));
            // Debug.DrawLine(parentPosition, thisPosition, Color.blue, 100000);

            foreach (Vertex<TType> forwardNeighbour in ForwardNeighbours)
            {
                if (forwardNeighbour is DrawableVertex<TType> draw)
                {
                    draw.Paint(thisPosition, drawPositions, dictionary);
                }
            }

            return drawPositions;
        }

        public class ListElement
        {
            public DrawableVertex<TType> t;
            public Vector3 parent;
            public Vector3 tPosition;

            public ListElement(DrawableVertex<TType> t, Vector3 parent, Vector3 tPosition)
            {
                this.t = t;
                this.parent = parent;
                this.tPosition = tPosition;
            }
        }
    }
}