using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.GraphGrammar
{
    public class Vertex<TType>
    {
        public IList<Vertex<TType>> ForwardNeighbours { get; }
        public IList<Vertex<TType>> IncomingNeighbours { get; }
        public TType Type { get; set; }
        public bool Discovered { get; set; }

        public Vertex(TType type)
        {
            this.Type = type;
            ForwardNeighbours = new List<Vertex<TType>>();
            IncomingNeighbours = new List<Vertex<TType>>();
        }

        public void AddNextNeighbour(Vertex<TType> neighbour)
        {
            ForwardNeighbours.Add(neighbour);
            neighbour.IncomingNeighbours.Add(this);
        }

        public void RemoveNextNeighbour(Vertex<TType> neighbour)
        {
            ForwardNeighbours.Remove(neighbour);
            neighbour.IncomingNeighbours.Remove(this);
        }

        public void AddPreviousNeighbour(Vertex<TType> neighbour)
        {
            IncomingNeighbours.Add(neighbour);
            neighbour.ForwardNeighbours.Add(this);
        }

        public void RemovePreviousNeighbour(Vertex<TType> neighbour)
        {
            IncomingNeighbours.Remove(neighbour);
            neighbour.ForwardNeighbours.Remove(this);
        }

        public void RemoveFromAllNeighbours()
        {
            foreach (Vertex<TType> neighbour in ForwardNeighbours)
            {
                neighbour.IncomingNeighbours.Remove(this);
            }

            foreach (Vertex<TType> neighbour in IncomingNeighbours)
            {
                neighbour.ForwardNeighbours.Remove(this);
            }
        }

        protected bool Equals(Vertex<TType> vertex)
        {
            return Type.Equals(vertex.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Vertex<TType>) obj);
        }

        public override string ToString()
        {
            return $"Vertex: {Type}";
        }


        public void TransferIncomingEdges(Vertex<TType> same)
        {
            foreach (Vertex<TType> vertex in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(vertex);
                same.AddPreviousNeighbour(vertex);
            }
        }

        public void TransferOutgoingEdges(Vertex<TType> same)
        {
            foreach (Vertex<TType> vertex in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(vertex);
                same.AddNextNeighbour(vertex);
            }
        }
    }
}