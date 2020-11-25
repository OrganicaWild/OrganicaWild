using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Demo.GraphGrammar;
using Framework.Util;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;


namespace Framework.GraphGrammar
{
    [Serializable]
    public class Graph<TType> where TType : ITerminality
    {
        public ISet<Vertex<TType>> Vertices { get; set; }
        public Vertex<TType> Start { get; set; }
        public Vertex<TType> End { get; set; }

        public Graph()
        {
            Vertices = new HashSet<Vertex<TType>>();
        }

        public Graph<TType> Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Graph<TType>) formatter.Deserialize(ms);
            }
        }

        public void AddVertex(Vertex<TType> vertex)
        {
            Vertices.Add(vertex);
        }

        public Vertex<TType> AddVertex(TType type)
        {
            Vertex<TType> vertex = new Vertex<TType>(type);
            Vertices.Add(vertex);
            return vertex;
        }

        public void RemoveVertex(Vertex<TType> vertex)
        {
            vertex.RemoveFromAllNeighbours();
            Vertices.Remove(vertex);
        }

        public bool Contains(Graph<TType> graph)
        {
            List<Graph<TType>> containedAt = ContainedSubGraph(graph);
            return containedAt.Count > 0;
        }

        public List<Graph<TType>> ContainedSubGraph(Graph<TType> graph)
        {
            List<MutableTuple<Vertex<TType>>> potentialPositions
                = Vertices.Where(x => x.Equals(graph.Start))
                    .Select(x => new MutableTuple<Vertex<TType>>(x, null)).ToList();
            List<Graph<TType>> potentialSubGraphs = new List<Graph<TType>>();

            foreach (MutableTuple<Vertex<TType>> potentialPosition in potentialPositions.ToArray())
            {
                Graph<TType> subGraph = new Graph<TType> {Start = potentialPosition.Item1};
                potentialSubGraphs.Add(subGraph);
                List<Tuple<Vertex<TType>, Vertex<TType>>> pairs = new List<Tuple<Vertex<TType>, Vertex<TType>>>()
                    {new Tuple<Vertex<TType>, Vertex<TType>>(graph.Start, potentialPosition.Item1)};

                while (pairs.Any())
                {
                    Tuple<Vertex<TType>, Vertex<TType>> currentPair = pairs[0];

                    pairs.Remove(currentPair);
                    Vertex<TType> subGraphNode = currentPair.Item1;
                    Vertex<TType> graphNode = currentPair.Item2;

                    subGraph.AddVertex(graphNode);

                    if (graphNode.Equals(graph.End))
                    {
                        potentialPosition.Item2 = graphNode;
                        subGraph.End = graphNode;
                    }

                    List<Vertex<TType>> allContainedNeighbours = new List<Vertex<TType>>();
                    List<Vertex<TType>> graphNeighbourCopy = graphNode.ForwardNeighbours
                        .OrderBy(vertex => vertex.ForwardNeighbours.Count).ToList();

                    foreach (Vertex<TType> subGraphNeighbour in subGraphNode.ForwardNeighbours.OrderBy(vertex =>
                        vertex.ForwardNeighbours.Count))
                    {
                        if (graphNeighbourCopy.Contains(subGraphNeighbour))
                        {
                            Vertex<TType> neighbour = graphNeighbourCopy.Find(v => v.Equals(subGraphNeighbour));
                            graphNeighbourCopy.Remove(neighbour);
                            allContainedNeighbours.Add(neighbour);
                        }
                    }

                    bool hasAllNeighbours = allContainedNeighbours.Count() ==
                                            subGraphNode.ForwardNeighbours.Count;

                    if (hasAllNeighbours)
                    {
                        List<Tuple<Vertex<TType>, Vertex<TType>>> newPairs =
                            new List<Tuple<Vertex<TType>, Vertex<TType>>>();
                        List<Vertex<TType>> graphNeighbourCopy1 = graphNode.ForwardNeighbours
                            .OrderBy(vertex => vertex.ForwardNeighbours.Count).ToList();

                        foreach (Vertex<TType> subGraphNeighbour in subGraphNode.ForwardNeighbours.OrderBy(vertex =>
                            vertex.ForwardNeighbours.Count))
                        {
                            if (graphNeighbourCopy1.Contains(subGraphNeighbour))
                            {
                                Vertex<TType> neighbour = graphNeighbourCopy1.Find(v => v.Equals(subGraphNeighbour));
                                graphNeighbourCopy1.Remove(neighbour);
                                newPairs.Add(new Tuple<Vertex<TType>, Vertex<TType>>(subGraphNeighbour, neighbour));
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

        public List<Vertex<TType>> Traverse()
        {
            List<Vertex<TType>> traversal = new List<Vertex<TType>>();
            Queue<Vertex<TType>> q = new Queue<Vertex<TType>>();
            HashSet<Vertex<TType>> prevEnqueued = new HashSet<Vertex<TType>>();
            q.Enqueue(Start);
            prevEnqueued.Add(Start);

            while (q.Any())
            {
                Vertex<TType> v = q.Dequeue();

                traversal.Add(v);

                foreach (Vertex<TType> vForwardNeighbour in v.ForwardNeighbours)
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