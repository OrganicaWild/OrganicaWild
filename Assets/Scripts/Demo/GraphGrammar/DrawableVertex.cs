using System;
using System.Collections.Generic;
using Framework.GraphGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.GraphGrammar
{
    public class DrawableVertex : Vertex
    {
        public DrawableVertex(int type) : base(type)
        {
        }

        public List<ListElement> Paint(Vector3 parentPosition,
            List<ListElement> drawPositions)
        {
            float xRandom = Random.value;
            float zRandom = Random.value;

            Vector3 thisPosition = parentPosition + new Vector3(xRandom, 0, zRandom).normalized;
            
            drawPositions.Add(new ListElement(this, parentPosition, thisPosition));

            // Debug.DrawLine(parentPosition, thisPosition, Color.blue, 100000);

            foreach (Vertex forwardNeighbour in ForwardNeighbours)
            {
                DrawableVertex draw = forwardNeighbour as DrawableVertex;
                if (draw != null)
                {
                    draw.Paint(thisPosition, drawPositions);
                }
            }

            return drawPositions;
        }
        
        public class ListElement
        {
            public DrawableVertex t;
            public Vector3 parent;
            public Vector3 tPosition;

            public ListElement(DrawableVertex t, Vector3 parent, Vector3 tPosition)
            {
                this.t = t;
                this.parent = parent;
                this.tPosition = tPosition;
            }
        }
    }
}