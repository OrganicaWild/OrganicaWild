using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Framework.Util;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;


namespace Framework.GraphGrammar
{
    [Serializable]
    public class Graph<TType> where TType : ITerminality
    {
        public IList<Vertex<TType>> Vertices { get; }
        public Vertex<TType> Start { get; set; }
        public Vertex<TType> End { get; set; }

        public Graph()
        {
            Vertices = new List<Vertex<TType>>();
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
            List<MutableTuple<Vertex<TType>>> containedAt = ContainedAt(graph);
            return containedAt.Count > 0;
        }

        public List<MutableTuple<Vertex<TType>>> ContainedAt(Graph<TType> graph)
        {
            var potentialPositions
                = Vertices.Where(x => x.Equals(graph.Start))
                    .Select(x => new MutableTuple<Vertex<TType>>(x, null)).ToList();

            foreach (MutableTuple<Vertex<TType>> potentialPosition in potentialPositions.ToArray())
            {
                var pairs = new List<Tuple<Vertex<TType>, Vertex<TType>>>()
                    {new Tuple<Vertex<TType>, Vertex<TType>>(graph.Start, potentialPosition.Item1)};

                while (pairs.Any())
                {
                    var currentPair = pairs[0];
                    pairs.Remove(currentPair);
                    Vertex<TType> subGraphNode = currentPair.Item1;
                    var graphNode = currentPair.Item2;

                    if (graphNode.Equals(graph.End))
                    {
                        potentialPosition.Item2 = graphNode;
                    }

                    var allContainedNeighbours =
                        subGraphNode.ForwardNeighbours.Where(subGraphNeighbour =>
                            graphNode.ForwardNeighbours.Contains(subGraphNeighbour));

                    var hasAllNeighbours = allContainedNeighbours.Count() ==
                                           subGraphNode.ForwardNeighbours.Count;

                    if (hasAllNeighbours)
                    {
                        var newPairs = subGraphNode.ForwardNeighbours.Select(x =>
                            new Tuple<Vertex<TType>, Vertex<TType>>(x,
                                graphNode.ForwardNeighbours.First(y => y.Equals(x))));
                        pairs.AddRange(newPairs);
                    }
                    else
                    {
                        potentialPositions.Remove(potentialPosition);
                        break;
                    }
                }
            }
            

            return potentialPositions;
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