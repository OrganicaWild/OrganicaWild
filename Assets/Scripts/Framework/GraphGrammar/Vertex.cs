using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.GraphGrammar
{
    [Serializable]
    public class Vertex<TType> : IEquatable<Vertex<TType>>
    {
        public IList<Vertex<TType>> ForwardNeighbours { get; }
        public IList<Vertex<TType>> IncomingNeighbours { get; }
        public TType Type { get; }
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

        public bool Equals(Vertex<TType> vertex)
        {
            return vertex != null && Type.Equals(vertex.Type);
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

        public override int GetHashCode()
        {
            return EqualityComparer<TType>.Default.GetHashCode(Type);
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

        public void TransferAllEdgesExceptOne(Vertex<TType> same, Vertex<TType> theOne)
        {
            foreach (Vertex<TType> vertex in ForwardNeighbours.ToArray())
            {
                if (vertex != theOne)
                {
                    RemoveNextNeighbour(vertex);
                    same.AddNextNeighbour(vertex);
                }
            }

            foreach (Vertex<TType> vertex in IncomingNeighbours.ToArray())
            {
                if (vertex != theOne)
                {
                    RemovePreviousNeighbour(vertex);
                    same.AddPreviousNeighbour(vertex);
                }
            }
        }

        public void RemoveIncomingEdges()
        {
            foreach (Vertex<TType> incomingNeighbour in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(incomingNeighbour);
            }
        }

        public void RemoveOutgoingEdges()
        {
            foreach (Vertex<TType> forwardNeighbour in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(forwardNeighbour);
            }
        }
    }
}