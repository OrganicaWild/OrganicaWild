using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
                    foreach (Vertex<TType> u in  v.ForwardNeighbours)
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
            IList<Vertex<TType>> thisDfs = Dfs();
            IList<Vertex<TType>> otherDfs = graph.Dfs();

            return Enumerable
                .Range(0, thisDfs.Count - otherDfs.Count + 1)
                .Any(n => thisDfs.Skip(n).Take(otherDfs.Count).SequenceEqual(otherDfs));
        }

        public override string ToString()
        {
            return $"({string.Join(", ", Vertices)})";
        }
    }
}