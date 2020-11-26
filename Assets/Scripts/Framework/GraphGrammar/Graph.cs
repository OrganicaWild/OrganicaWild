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
    public class Graph
    {
        public ISet<Vertex> Vertices { get; set; }
        public Vertex Start { get; set; }
        public Vertex End { get; set; }

        public Graph()
        {
            Vertices = new HashSet<Vertex>();
        }

        public Graph Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Graph) formatter.Deserialize(ms);
            }
        }

        public void AddVertex(Vertex vertex)
        {
            Vertices.Add(vertex);
        }

        public Vertex AddVertex(string type)
        {
            Vertex vertex = new Vertex(type);
            Vertices.Add(vertex);
            return vertex;
        }

        public void RemoveVertex(Vertex vertex)
        {
            vertex.RemoveFromAllNeighbours();
            Vertices.Remove(vertex);
        }

        public bool Contains(Graph graph)
        {
            List<Graph> containedAt = ContainedSubGraph(graph);
            return containedAt.Count > 0;
        }

        public List<Graph> ContainedSubGraph(Graph graph)
        {
            List<MutableTuple<Vertex>> potentialPositions
                = Vertices.Where(x => x.Equals(graph.Start))
                    .Select(x => new MutableTuple<Vertex>(x, null)).ToList();
            List<Graph> potentialSubGraphs = new List<Graph>();

            foreach (MutableTuple<Vertex> potentialPosition in potentialPositions.ToArray())
            {
                Graph subGraph = new Graph {Start = potentialPosition.Item1};
                potentialSubGraphs.Add(subGraph);
                List<Tuple<Vertex, Vertex>> pairs = new List<Tuple<Vertex, Vertex>>()
                    {new Tuple<Vertex, Vertex>(graph.Start, potentialPosition.Item1)};

                while (pairs.Any())
                {
                    Tuple<Vertex, Vertex> currentPair = pairs[0];

                    pairs.Remove(currentPair);
                    Vertex subGraphNode = currentPair.Item1;
                    Vertex graphNode = currentPair.Item2;

                    subGraph.AddVertex(graphNode);

                    if (graphNode.Equals(graph.End))
                    {
                        potentialPosition.Item2 = graphNode;
                        subGraph.End = graphNode;
                    }

                    List<Vertex> allContainedNeighbours = new List<Vertex>();
                    List<Vertex> graphNeighbourCopy = graphNode.ForwardNeighbours
                        .OrderBy(vertex => vertex.ForwardNeighbours.Count).ToList();

                    foreach (Vertex subGraphNeighbour in subGraphNode.ForwardNeighbours.OrderBy(vertex =>
                        vertex.ForwardNeighbours.Count))
                    {
                        if (graphNeighbourCopy.Contains(subGraphNeighbour))
                        {
                            Vertex neighbour = graphNeighbourCopy.Find(v => v.Equals(subGraphNeighbour));
                            graphNeighbourCopy.Remove(neighbour);
                            allContainedNeighbours.Add(neighbour);
                        }
                    }

                    bool hasAllNeighbours = allContainedNeighbours.Count() ==
                                            subGraphNode.ForwardNeighbours.Count;

                    if (hasAllNeighbours)
                    {
                        List<Tuple<Vertex, Vertex>> newPairs =
                            new List<Tuple<Vertex, Vertex>>();
                        List<Vertex> graphNeighbourCopy1 = graphNode.ForwardNeighbours
                            .OrderBy(vertex => vertex.ForwardNeighbours.Count).ToList();

                        foreach (Vertex subGraphNeighbour in subGraphNode.ForwardNeighbours.OrderBy(vertex =>
                            vertex.ForwardNeighbours.Count))
                        {
                            if (graphNeighbourCopy1.Contains(subGraphNeighbour))
                            {
                                Vertex neighbour = graphNeighbourCopy1.Find(v => v.Equals(subGraphNeighbour));
                                graphNeighbourCopy1.Remove(neighbour);
                                newPairs.Add(new Tuple<Vertex, Vertex>(subGraphNeighbour, neighbour));
                            }
                        }

                        pairs.AddRange(newPairs);
                    }
                    else
                    {
                        potentialPositions.Remove(potentialPosition);
                        potentialSubGraphs.Remove(subGraph);
                        break;
                    }
                }
            }


            return potentialSubGraphs;
        }

        public List<Vertex> Traverse()
        {
            List<Vertex> traversal = new List<Vertex>();
            Queue<Vertex> q = new Queue<Vertex>();
            HashSet<Vertex> prevEnqueued = new HashSet<Vertex>();
            q.Enqueue(Start);
            prevEnqueued.Add(Start);

            while (q.Any())
            {
                Vertex v = q.Dequeue();

                traversal.Add(v);

                foreach (Vertex vForwardNeighbour in v.ForwardNeighbours)
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