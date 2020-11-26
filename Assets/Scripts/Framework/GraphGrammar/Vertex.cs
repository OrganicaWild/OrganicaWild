using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.GraphGrammar
{
    [Serializable]
    public class Vertex : IEquatable<Vertex>
    {
        public IList<Vertex> ForwardNeighbours { get; }
        public IList<Vertex> IncomingNeighbours { get; }
        public string Type { get; }
        public bool Discovered { get; set; }

        public Vertex(string type)
        {
            this.Type = type;
            ForwardNeighbours = new List<Vertex>();
            IncomingNeighbours = new List<Vertex>();
        }

        public void AddNextNeighbour(Vertex neighbour)
        {
            ForwardNeighbours.Add(neighbour);
            neighbour.IncomingNeighbours.Add(this);
        }

        public void RemoveNextNeighbour(Vertex neighbour)
        {
            ForwardNeighbours.Remove(neighbour);
            neighbour.IncomingNeighbours.Remove(this);
        }

        public void AddPreviousNeighbour(Vertex neighbour)
        {
            IncomingNeighbours.Add(neighbour);
            neighbour.ForwardNeighbours.Add(this);
        }

        public void RemovePreviousNeighbour(Vertex neighbour)
        {
            IncomingNeighbours.Remove(neighbour);
            neighbour.ForwardNeighbours.Remove(this);
        }

        public void RemoveFromAllNeighbours()
        {
            foreach (Vertex neighbour in ForwardNeighbours)
            {
                neighbour.IncomingNeighbours.Remove(this);
            }

            foreach (Vertex neighbour in IncomingNeighbours)
            {
                neighbour.ForwardNeighbours.Remove(this);
            }
        }

        public bool Equals(Vertex vertex)
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

            return Equals((Vertex) obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Vertex: {Type}";
        }

        public void TransferIncomingEdges(Vertex same)
        {
            foreach (Vertex vertex in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(vertex);
                same.AddPreviousNeighbour(vertex);
            }
        }

        public void TransferOutgoingEdges(Vertex same)
        {
            foreach (Vertex vertex in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(vertex);
                same.AddNextNeighbour(vertex);
            }
        }

        public void TransferAllEdgesExceptOne(Vertex same, Vertex theOne)
        {
            foreach (Vertex vertex in ForwardNeighbours.ToArray())
            {
                if (vertex != theOne)
                {
                    RemoveNextNeighbour(vertex);
                    same.AddNextNeighbour(vertex);
                }
            }

            foreach (Vertex vertex in IncomingNeighbours.ToArray())
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
            foreach (Vertex incomingNeighbour in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(incomingNeighbour);
            }
        }

        public void RemoveOutgoingEdges()
        {
            foreach (Vertex forwardNeighbour in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(forwardNeighbour);
            }
        }
    }
}