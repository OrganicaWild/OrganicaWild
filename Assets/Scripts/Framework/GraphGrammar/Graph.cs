using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Random = UnityEngine.Random;


namespace Framework.GraphGrammar
{
    [Serializable]
    public class Graph<TType>
    {
        
        private IList<Vertex<TType>> vertices;
        public Vertex<TType> start { get; set; }
        public Vertex<TType> end { get; set; }

        public Graph()
        {
            vertices = new List<Vertex<TType>>();
        }

        private Graph<TType> Clone()
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
            vertices.Add(vertex);
        }

        public Vertex<TType> AddVertex(TType type)
        {
            Vertex<TType> vertex = new Vertex<TType>(type);
            vertices.Add(vertex);
            return vertex;
        }

        public void RemoveVertex(Vertex<TType> vertex)
        {
            vertex.RemoveFromAllNeighbours();
            vertices.Remove(vertex);
        }

        public IList<Vertex<TType>> Dfs()
        {
            foreach (Vertex<TType> vertex in vertices)
            {
                vertex.Discovered = false;
            }

            IList<Vertex<TType>> traversalQ = new List<Vertex<TType>>();
            Stack<Vertex<TType>> s = new Stack<Vertex<TType>>();
            s.Push(start);

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

            return traversalQ;
        }

        public bool Contains(Graph<TType> graph)
        {
            IList<Vertex<TType>> thisDfs = Dfs();
            IList<Vertex<TType>> otherDfs = Dfs();

            return Enumerable
                .Range(0, thisDfs.Count - otherDfs.Count + 1)
                .Any(n => thisDfs.Skip(n).Take(otherDfs.Count).SequenceEqual(otherDfs));
        }

        public bool ReplaceSubGraph(GrammarRule<TType> rule)
        {
            IList<Vertex<TType>> thisDfs = Dfs();
            IList<Vertex<TType>> otherDfs = rule.LeftHandSide.Dfs();

            //find all possible startPositions
            int[] starts = Enumerable
                .Range(0, thisDfs.Count - otherDfs.Count + 1)
                .Where(n => thisDfs.Skip(n).Take(otherDfs.Count).SequenceEqual(otherDfs)).ToArray();

            if (!starts.Any())
            {
                return false;
            }

            //chose start position
            int chosenIndex = Random.Range(0, starts.Count());
            Vertex<TType> chosenStart = thisDfs[starts[chosenIndex]];
            Vertex<TType> chosenEnd = thisDfs[starts[chosenIndex] + otherDfs.Count - 1];

            //chosenStart.AddNextNeighbour(rule.RightHandSide.start);
            Graph<TType> rightHandSideCopy = rule.RightHandSide.Clone();
            
            chosenStart.TransferIncomingEdges(rightHandSideCopy.start);
            chosenEnd.TransferOutgoingEdges(rightHandSideCopy.end);

            //if we replace start, we gotta switch the start reference
            if (chosenStart == start) 
            {
                start = rightHandSideCopy.start;
            }

            //if we replace end, we gotta switch the end reference
            if (chosenEnd == end)
            {
                end = rightHandSideCopy.end;
            }
            
            return true;
        }
    }
}