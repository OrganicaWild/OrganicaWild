using System;
using System.Collections.Generic;

namespace Framework.GraphGrammar
{
    /// <summary>
    /// Provides a single MissionVertex. Generally used with a MissionGraph.
    /// </summary>
    [Serializable]
    public class MissionVertex : IEquatable<MissionVertex>
    {
        /// <summary>
        /// List of all neighbours where an edge leads from this MissionVertex to that MissionVertex .
        /// </summary>
        public List<MissionVertex> ForwardNeighbours { get; }

        /// <summary>
        /// List of all neighbours where an edge leads from that MissionVertex to this MissionVertex.
        /// </summary>
        public List<MissionVertex> IncomingNeighbours { get; }

        /// <summary>
        /// Type of the MissionVertex.
        /// What part of the mission does this MissionVertex represent?
        /// Common Types may be.: Entrance, HealthPack, Boss, Key ...
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Create a new Mission Vertex with the specified Type
        /// </summary>
        /// <param name="type">Type of the mission part</param>
        public MissionVertex(string type)
        {
            Type = type;
            ForwardNeighbours = new List<MissionVertex>();
            IncomingNeighbours = new List<MissionVertex>();
        }

        /// <summary>
        /// For Serialization
        /// </summary>
        private MissionVertex()
        {
        }

        /// <summary>
        /// Add a new edge from this MissionVertex to that MissionVertex.
        /// This method also adds the twin edge from that to this.
        /// </summary>
        /// <param name="neighbour">that MissionVertex</param>
        public void AddNextNeighbour(MissionVertex neighbour)
        {
            ForwardNeighbours.Add(neighbour);
            neighbour.IncomingNeighbours.Add(this);
        }

        /// <summary>
        /// Remove a new edge from this MissionVertex to that MissionVertex.
        /// This method also removes the twin edge from that to this.
        /// </summary>
        /// <param name="neighbour">that MissionVertex</param>
        public void RemoveNextNeighbour(MissionVertex neighbour)
        {
            ForwardNeighbours.Remove(neighbour);
            neighbour.IncomingNeighbours.Remove(this);
        }

        /// <summary>
        /// Add a new edge from that MissionVertex to this MissionVertex.
        /// This method also adds the twin edge from this to that.
        /// </summary>
        /// <param name="neighbour">that MissionVertex</param>
        public void AddPreviousNeighbour(MissionVertex neighbour)
        {
            IncomingNeighbours.Add(neighbour);
            neighbour.ForwardNeighbours.Add(this);
        }

        /// <summary>
        /// Remove a new edge from that MissionVertex to this MissionVertex.
        /// This method also removes the twin edge from this to that.
        /// </summary>
        /// <param name="neighbour">that MissionVertex</param>
        public void RemovePreviousNeighbour(MissionVertex neighbour)
        {
            IncomingNeighbours.Remove(neighbour);
            neighbour.ForwardNeighbours.Remove(this);
        }

        /// <summary>
        /// Removes all edges from this MissionVertex.
        /// This Method also cleans up all twin edges.
        /// When removing a MissionVertex from a MissionGraph with RemoveVertex(), this method is also executed.
        /// </summary>
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

        /// <summary>
        /// Transfer all edges of this MissionVertex, that are coming from other MissionVertex's to this to the supplied MissionVertex.
        /// All twin edges are also all transferred.
        /// </summary>
        /// <param name="same">to transfer to with same Type</param>
        internal void TransferIncomingEdges(MissionVertex same)
        {
            foreach (MissionVertex vertex in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(vertex);
                same.AddPreviousNeighbour(vertex);
            }
        }

        /// <summary>
        /// Transfer all edges of this MissionVertex, that are going from this to any other MissionVertex's to this supplied MissionVertex.
        /// All twin edges are also transferred.
        /// </summary>
        /// <param name="same">to transfer to with same Type</param>
        internal void TransferOutgoingEdges(MissionVertex same)
        {
            foreach (MissionVertex vertex in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(vertex);
                same.AddNextNeighbour(vertex);
            }
        }

        /// <summary>
        /// Transfer all edges from and to this MissionVertex to the given MissionVertex same.
        /// Except for all edges connected to the MissionVertex theOne.
        /// </summary>
        /// <param name="same">to transfer to with same Type</param>
        /// <param name="theOne">all edges to or from this MissionVertex are not transferred</param>
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

        internal void RemoveIncomingEdges()
        {
            foreach (MissionVertex incomingNeighbour in IncomingNeighbours.ToArray())
            {
                RemovePreviousNeighbour(incomingNeighbour);
            }
        }

        internal void RemoveOutgoingEdges()
        {
            foreach (MissionVertex forwardNeighbour in ForwardNeighbours.ToArray())
            {
                RemoveNextNeighbour(forwardNeighbour);
            }
        }

        /// <summary>
        /// EqualityComparer for MissionVertex between this and other.
        /// Two MissionVertex are equal, if their Type is the same.
        /// </summary>
        /// <param name="missionVertex">other</param>
        /// <returns>true, if same. false, if not</returns>
        public bool Equals(MissionVertex missionVertex)
        {
            return missionVertex != null && Type.Equals(missionVertex.Type);
        }

        /// <summary>
        /// EqualityComparer for MissionVertex and any object.
        /// Are only equal if both are MissionVertex and their Type is the same.
        /// </summary>
        /// <param name="obj">other</param>
        /// <returns>true, if same. false, if not</returns>
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
    }
}