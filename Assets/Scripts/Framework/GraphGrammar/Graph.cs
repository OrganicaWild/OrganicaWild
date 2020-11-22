using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


namespace Framework.GraphGrammar
{
    public class Graph
    {
        private IList<Vertex> vertices;
        public Vertex start { get; set; }
        public Vertex end { get; set; }

        public Graph()
        {
            vertices = new List<Vertex>();
        }

        public void AddVertex(Vertex vertex)
        {
            vertices.Add(vertex);
        }

        public Vertex AddVertex(int type)
        {
            Vertex vertex = new Vertex(type);
            vertices.Add(vertex);
            return vertex;
        }

        public void RemoveVertex(Vertex vertex)
        {
            vertex.RemoveFromAllNeighbours();
            vertices.Remove(vertex);
        }

        public IList<Vertex> Dfs()
        {
            foreach (Vertex vertex in vertices)
            {
                vertex.Discovered = false;
            }

            IList<Vertex> traversalQ = new List<Vertex>();
            Stack<Vertex> s = new Stack<Vertex>();
            s.Push(start);

            while (s.Any())
            {
                Vertex v = s.Pop();
                traversalQ.Add(v);
                if (!v.Discovered)
                {
                    v.Discovered = true;
                    foreach (Vertex u in v.ForwardNeighbours)
                    {
                        s.Push(u);
                    }
                }
            }

            return traversalQ;
        }

        public bool Contains(Graph graph)
        {
            IList<Vertex> thisDfs = Dfs();
            IList<Vertex> otherDfs = Dfs();

            return Enumerable
                .Range(0, thisDfs.Count - otherDfs.Count + 1)
                .Any(n => thisDfs.Skip(n).Take(otherDfs.Count).SequenceEqual(otherDfs));
        }

        public bool ReplaceSubGraph(GrammarRule rule)
        {
            IList<Vertex> thisDfs = Dfs();
            IList<Vertex> otherDfs = rule.LeftHandSide.Dfs();

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
            Vertex chosenStart = thisDfs[starts[chosenIndex]];
            Vertex chosenEnd = thisDfs[starts[chosenIndex] + otherDfs.Count - 1];

            //chosenStart.AddNextNeighbour(rule.RightHandSide.start);
            
            chosenStart.TransferIncomingEdges(rule.RightHandSide.start);
            chosenEnd.TransferOutgoingEdges(rule.RightHandSide.end);

            //if we replace start, we gotta switch the start reference
            if (chosenStart == start) 
            {
                start = rule.RightHandSide.start;
            }

            //if we replace end, we gotta switch the end reference
            if (chosenEnd == end)
            {
                end = rule.RightHandSide.end;
            }
            
            return true;
        }
    }
}