using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Framework.Util;

namespace Framework.GraphGrammar
{
    /// <summary>
    /// Provides a graph structure for missions
    /// </summary>
    [Serializable]
    public class MissionGraph
    {
        /// <summary>
        /// All vertices of the graph in a readonly list
        /// </summary>
        public List<MissionVertex> Vertices { get; }

        /// <summary>
        /// The Start vertex of the mission graph
        /// </summary>
        public MissionVertex Start { get; set; }

        /// <summary>
        /// The End vertex of the mission graph
        /// </summary>
        public MissionVertex End { get; set; }

        public MissionGraph()
        {
            Vertices = new List<MissionVertex>();
        }

        /// <summary>
        /// Clone the mission graph
        /// </summary>
        /// <returns>A clone of this</returns>
        public MissionGraph Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (MissionGraph) formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Add a vertex to a graph
        /// </summary>
        /// <param name="missionVertex">vertex to be added</param>
        public void AddVertex(MissionVertex missionVertex)
        {
            Vertices.Add(missionVertex);
        }

        /// <summary>
        /// Create and add a new Vertex by type to this MissionGraph.
        /// </summary>
        /// <param name="type">type of vertex</param>
        /// <returns>A new MissionVertex instance with the specified type</returns>
        public MissionVertex AddVertex(string type)
        {
            MissionVertex missionVertex = new MissionVertex(type);
            Vertices.Add(missionVertex);
            return missionVertex;
        }

        /// <summary>
        /// Removes a vertex from the MissionGraph.
        /// Uncoupling from other vertices is also done here and does not have to be done manually.
        /// </summary>
        /// <param name="missionVertex"></param>
        public void RemoveVertex(MissionVertex missionVertex)
        {
            missionVertex.RemoveFromAllNeighbours();
            Vertices.Remove(missionVertex);
        }

        /// <summary>
        /// Check whether a given MissionGraph is contained in this MissionGraph.
        /// </summary>
        /// <param name="missionGraph">MissionGraph to test if it is a subgraph</param>
        /// <returns>   true - missionGraph is subgraph
        ///             false - missionGraph is not subgraph</returns>
        public bool ContainsSubGraphBool(MissionGraph missionGraph)
        {
            List<MissionGraph> containedAt = ContainsSubGraphMultiple(missionGraph);
            return containedAt.Count > 0;
        }

        /// <summary>
        /// Check whether a given MissionGraph is contained in this MissionGraph
        /// </summary>
        /// <param name="missionGraph"></param>
        /// <returns>A list of all the Sub-Graphs in the mother graph, that have the same structure as the supplied MissionGraph</returns>
        internal List<MissionGraph> ContainsSubGraphMultiple(MissionGraph missionGraph)
        {
            List<MutableTuple<MissionVertex>> potentialPositions
                = Vertices.Where(x => x.Equals(missionGraph.Start))
                    .Select(x => new MutableTuple<MissionVertex>(x, null)).ToList();
            List<MissionGraph> potentialSubGraphs = new List<MissionGraph>();

            foreach (MutableTuple<MissionVertex> potentialPosition in potentialPositions.ToArray())
            {
                MissionGraph subMissionGraph = new MissionGraph {Start = potentialPosition.Item1};
                potentialSubGraphs.Add(subMissionGraph);
                List<Tuple<MissionVertex, MissionVertex>> pairs = new List<Tuple<MissionVertex, MissionVertex>>
                    {new Tuple<MissionVertex, MissionVertex>(missionGraph.Start, potentialPosition.Item1)};

                while (pairs.Any())
                {
                    Tuple<MissionVertex, MissionVertex> currentPair = pairs[0];

                    pairs.Remove(currentPair);
                    MissionVertex subGraphNode = currentPair.Item1;
                    MissionVertex graphNode = currentPair.Item2;

                    subMissionGraph.AddVertex(graphNode);

                    if (graphNode.Equals(missionGraph.End))
                    {
                        potentialPosition.Item2 = graphNode;
                        subMissionGraph.End = graphNode;
                    }

                    List<MissionVertex> allContainedNeighbours = new List<MissionVertex>();
                    List<MissionVertex> graphNeighbourCopy = graphNode.ForwardNeighbours
                        .OrderBy(vertex => vertex.ForwardNeighbours.Count).ToList();

                    foreach (MissionVertex subGraphNeighbour in subGraphNode.ForwardNeighbours.OrderBy(vertex =>
                        vertex.ForwardNeighbours.Count))
                    {
                        if (graphNeighbourCopy.Contains(subGraphNeighbour))
                        {
                            MissionVertex neighbour = graphNeighbourCopy.Find(v => v.Equals(subGraphNeighbour));
                            graphNeighbourCopy.Remove(neighbour);
                            allContainedNeighbours.Add(neighbour);
                        }
                    }

                    bool hasAllNeighbours = allContainedNeighbours.Count ==
                                            subGraphNode.ForwardNeighbours.Count;

                    if (hasAllNeighbours)
                    {
                        List<Tuple<MissionVertex, MissionVertex>> newPairs =
                            new List<Tuple<MissionVertex, MissionVertex>>();
                        List<MissionVertex> graphNeighbourCopy1 = graphNode.ForwardNeighbours
                            .OrderBy(vertex => vertex.ForwardNeighbours.Count).ToList();

                        foreach (MissionVertex subGraphNeighbour in subGraphNode.ForwardNeighbours.OrderBy(vertex =>
                            vertex.ForwardNeighbours.Count))
                        {
                            if (graphNeighbourCopy1.Contains(subGraphNeighbour))
                            {
                                MissionVertex neighbour = graphNeighbourCopy1.Find(v => v.Equals(subGraphNeighbour));
                                graphNeighbourCopy1.Remove(neighbour);
                                newPairs.Add(new Tuple<MissionVertex, MissionVertex>(subGraphNeighbour, neighbour));
                            }
                        }

                        pairs.AddRange(newPairs);
                    }
                    else
                    {
                        potentialPositions.Remove(potentialPosition);
                        potentialSubGraphs.Remove(subMissionGraph);
                        break;
                    }
                }
            }


            return potentialSubGraphs;
        }

        /// <summary>
        /// Traverse MissionGraph by BFS.
        /// </summary>
        /// <returns>List of vertices in traversed order</returns>
        public List<MissionVertex> Traverse()
        {
            List<MissionVertex> traversal = new List<MissionVertex>();
            Queue<MissionVertex> q = new Queue<MissionVertex>();
            HashSet<MissionVertex> prevEnqueued = new HashSet<MissionVertex>();
            q.Enqueue(Start);
            prevEnqueued.Add(Start);

            while (q.Any())
            {
                MissionVertex v = q.Dequeue();

                traversal.Add(v);

                foreach (MissionVertex vForwardNeighbour in v.ForwardNeighbours)
                {
                    if (!prevEnqueued.Contains(vForwardNeighbour))
                    {
                        prevEnqueued.Add(vForwardNeighbour);
                        q.Enqueue(vForwardNeighbour);
                    }
                }
            }

            //bug! sometimes adding a new subgraph does not remove the old vertex, has to be fixed.
            //Debug.Assert(Vertices.Count == traversal.Count);

            return traversal;
        }

        public override string ToString()
        {
            if (Start != null)
            {
                return next(Start);
            }
           
            return "";
        }

        private string next(MissionVertex vertex)
        {
            string result = "";
            string concat = "";

            if (!vertex.ForwardNeighbours.Any())
            {
                return $"[{vertex.Type}]";
            }
            
            foreach (MissionVertex forwardNeighbour in vertex.ForwardNeighbours)
            {
                result += $"[{vertex.Type} --> ";
                result += $"{forwardNeighbour.Type}]";
                concat += next(forwardNeighbour);
                result += ", ";
            }

            //result.Remove(result.Length - 2, result.Length - 1);
            result += concat;

            return result;
        }
    }
}