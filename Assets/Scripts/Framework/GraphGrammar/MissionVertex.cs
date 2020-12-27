using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.GraphGrammar
{
    [Serializable]
    public class MissionVertex : IEquatable<MissionVertex>
    {
        public List<MissionVertex> ForwardNeighbours { get; }
        public List<MissionVertex> IncomingNeighbours { get; }
        public string Type { get; set; }
        public bool Discovered { get; set; }

        public MissionVertex(string type)
        {
            this.Type = type;
            ForwardNeighbours = new List<MissionVertex>();
            IncomingNeighbours = new List<MissionVertex>();
        }

        public void AddNextNeighbour(MissionVertex neighbour)
        {
            ForwardNeighbours.Add(neighbour);
            neighbour.IncomingNeighbours.Add(this);
        }

        public void RemoveNextNeighbour(MissionVertex neighbour)
        {
            ForwardNeighbours.Remove(neighbour);
            neighbour.IncomingNeighbours.Remove(this);
        }

        public void AddPreviousNeighbour(MissionVertex neighbour)
        {
            IncomingNeighbours.Add(neighbour);
            neighbour.ForwardNeighbours.Add(this);
        }

        public void RemovePreviousNeighbour(MissionVertex neighbour)
        {
            IncomingNeighbours.Remove(neighbour);
            neighbour.ForwardNeighbours.Remove(this);
        }

        public void RemoveFromAllNeighbours()
        {
            foreach (MissionVertex neighbour in ForwardNeighbours)
            {
                neighbour.IncomingNeighbours.Remove(this);
            }

            foreach (MissionVertex neighbour in IncomingNeighbours)
            {
                neighbour.ForwardNeighbours.Remove(this);
            }
        }

        public bool Equals(MissionVertex missionVertex)
        {
            return missionVertex != null && Type.Equals(missionVertex.Type);
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

            return Equals((MissionVertex) obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Vertex: {Type}";
        }

        public void TransferIncomingEdges(MissionVertex same)
        {
            foreach (MissionVertex vertex in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(vertex);
                same.AddPreviousNeighbour(vertex);
            }
        }

        public void TransferOutgoingEdges(MissionVertex same)
        {
            foreach (MissionVertex vertex in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(vertex);
                same.AddNextNeighbour(vertex);
            }
        }

        public void TransferAllEdgesExceptOne(MissionVertex same, MissionVertex theOne)
        {
            foreach (MissionVertex vertex in ForwardNeighbours.ToArray())
            {
                if (vertex != theOne)
                {
                    RemoveNextNeighbour(vertex);
                    same.AddNextNeighbour(vertex);
                }
            }

            foreach (MissionVertex vertex in IncomingNeighbours.ToArray())
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
            foreach (MissionVertex incomingNeighbour in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(incomingNeighbour);
            }
        }

        public void RemoveOutgoingEdges()
        {
            foreach (MissionVertex forwardNeighbour in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(forwardNeighbour);
            }
        }
    }
}