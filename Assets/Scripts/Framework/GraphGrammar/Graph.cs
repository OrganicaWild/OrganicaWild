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
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
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

        public IList<Vertex<TType>> Dfs()
        {
            IList<Vertex<TType>> traversalQ = new List<Vertex<TType>>();
            Stack<Vertex<TType>> s = new Stack<Vertex<TType>>();
            s.Push(Start);

            while (s.Any())
            {
                Vertex<TType> v = s.Pop();
                traversalQ.Add(v);
                if (!v.Discovered)
                {
                    v.Discovered = true;
                    foreach (Vertex<TType> u in v.ForwardNeighbours)
                    {
                        s.Push(u);
                    }
                }
            }

            foreach (Vertex<TType> vertex in Vertices)
            {
                vertex.Discovered = false;
            }

            return traversalQ;
        }

        public bool Contains(Graph<TType> graph)
        {
            List<Graph<TType>> containedAt = ContainedSubGraph(graph);
            return containedAt.Count > 0;
        }

        public List<Graph<TType>> ContainedSubGraph(Graph<TType> graph)
        {
            var potentialPositions
                = Vertices.Where(x => x.Equals(graph.Start))
                    .Select(x => new MutableTuple<Vertex<TType>>(x, null)).ToList();
            var potentialSubGraphs = new List<Graph<TType>>();

            foreach (MutableTuple<Vertex<TType>> potentialPosition in potentialPositions.ToArray())
            {

                var subGraph = new Graph<TType>();
                subGraph.Start = potentialPosition.Item1;
                potentialSubGraphs.Add(subGraph);
                var pairs = new List<Tuple<Vertex<TType>, Vertex<TType>>>()
                    {new Tuple<Vertex<TType>, Vertex<TType>>(graph.Start, potentialPosition.Item1)};

                while (pairs.Any())
                {
                    var currentPair = pairs[0];
                    Debug.Log($"currentPair: {currentPair.Item1} {currentPair.Item2}");
                    pairs.Remove(currentPair);
                    Vertex<TType> subGraphNode = currentPair.Item1;
                    var graphNode = currentPair.Item2;
                    
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
                            var neighbour = graphNeighbourCopy.Find(v => v.Equals(subGraphNeighbour));
                            graphNeighbourCopy.Remove(neighbour);
                            allContainedNeighbours.Add(neighbour);
                        }
                    }

                    Debug.Log($"At: {graphNode} has neighbours {string.Join("; ", allContainedNeighbours)}");
                    var hasAllNeighbours = allContainedNeighbours.Count() ==
                                           subGraphNode.ForwardNeighbours.Count;
                    Debug.Log($"{hasAllNeighbours}");
                    
                    if (hasAllNeighbours)
                    {
                        var newPairs = new List<Tuple<Vertex<TType>, Vertex<TType>>>();
                        List<Vertex<TType>> graphNeighbourCopy1 = graphNode.ForwardNeighbours
                            .OrderBy(vertex => vertex.ForwardNeighbours.Count).ToList();

                        foreach (Vertex<TType> subGraphNeighbour in subGraphNode.ForwardNeighbours.OrderBy(vertex =>
                            vertex.ForwardNeighbours.Count))
                        {
                            if (graphNeighbourCopy1.Contains(subGraphNeighbour))
                            {
                                var neighbour = graphNeighbourCopy1.Find(v => v.Equals(subGraphNeighbour));
                                graphNeighbourCopy1.Remove(neighbour);
                                newPairs.Add(new Tuple<Vertex<TType>, Vertex<TType>>(subGraphNeighbour, neighbour));
                            }
                        }

                        pairs.AddRange(newPairs);
                    }
                    else
                    {
                        Debug.Log($"but should have neighbours {string.Join(";", subGraphNode.ForwardNeighbours)}");
                        potentialPositions.Remove(potentialPosition);
                        potentialSubGraphs.Remove(subGraph);
                        break;
                    }
                }
            }
            

            return potentialSubGraphs;
            // return potentialPositions;
            // foreach (Vertex<TType> vertex in Vertices)
            // {
            //     vertex.Discovered = false;
            // }
            //
            // IList<Vertex<TType>> thisDfs = Dfs();
            // IList<Vertex<TType>> otherDfs = graph.Dfs();
            //
            // return Enumerable
            //     .Range(0, thisDfs.Count - otherDfs.Count + 1)
            //     .Any(n => thisDfs.Skip(n).Take(otherDfs.Count).SequenceEqual(otherDfs));
        }

        public override string ToString()
        {
            return $"({string.Join(", ", Vertices)})";
        }
    }
}