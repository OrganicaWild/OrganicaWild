using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Framework.Util;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;


namespace Framework.GraphGrammar
{
    [Serializable]
    public class MissionGraph
    {
        public ISet<MissionVertex> Vertices { get; set; }
        public MissionVertex Start { get; set; }
        public MissionVertex End { get; set; }

        public MissionGraph()
        {
            Vertices = new HashSet<MissionVertex>();
        }

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

        public void AddVertex(MissionVertex missionVertex)
        {
            Vertices.Add(missionVertex);
        }

        public MissionVertex AddVertex(string type)
        {
            MissionVertex missionVertex = new MissionVertex(type);
            Vertices.Add(missionVertex);
            return missionVertex;
        }

        public void RemoveVertex(MissionVertex missionVertex)
        {
            missionVertex.RemoveFromAllNeighbours();
            Vertices.Remove(missionVertex);
        }

        public bool Contains(MissionGraph missionGraph)
        {
            List<MissionGraph> containedAt = ContainedSubGraph(missionGraph);
            return containedAt.Count > 0;
        }

        public List<MissionGraph> ContainedSubGraph(MissionGraph missionGraph)
        {
            List<MutableTuple<MissionVertex>> potentialPositions
                = Vertices.Where(x => x.Equals(missionGraph.Start))
                    .Select(x => new MutableTuple<MissionVertex>(x, null)).ToList();
            List<MissionGraph> potentialSubGraphs = new List<MissionGraph>();

            foreach (MutableTuple<MissionVertex> potentialPosition in potentialPositions.ToArray())
            {
                MissionGraph subMissionGraph = new MissionGraph {Start = potentialPosition.Item1};
                potentialSubGraphs.Add(subMissionGraph);
                List<Tuple<MissionVertex, MissionVertex>> pairs = new List<Tuple<MissionVertex, MissionVertex>>()
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

                    bool hasAllNeighbours = allContainedNeighbours.Count() ==
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

            return traversal;
        }

        public override string ToString()
        {
            return $"({string.Join(", ", Vertices)})";
        }
    }
}