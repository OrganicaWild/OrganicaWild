using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Framework.GraphGrammar
{
    public class Vertex
    {
        public IList<Vertex> ForwardNeighbours { get; }
        public IList<Vertex> IncomingNeighbours { get; }
        public int Type { get; }
        public bool Discovered { get; set; }

        public Vertex(int type)
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

        protected bool Equals(Vertex vertex)
        {
            return Type == vertex.Type;
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

        public override string ToString()
        {
            return $"Vertex: {Type}";
        }

        public override int GetHashCode()
        {
            return Type;
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
    }
}