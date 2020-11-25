using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.GraphGrammar
{
    public class GraphGrammar<TType> where TType : ITerminality
    {
        private readonly IList<GrammarRule<TType>> rules;
        private readonly Graph<TType> mother;

        public GraphGrammar(IList<GrammarRule<TType>> rules, Graph<TType> mother)
        {
            this.rules = rules;
            this.mother = mother;
        }

        public Graph<TType> GetLevel()
        {
            return mother.Clone();
        }

        public int RuleCount()
        {
            return rules.Count;
        }

        public void ApplyOneRule()
        {
            GrammarRule<TType>[] workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            if (workingRules.Any())
            {
                GrammarRule<TType> chosenRule = workingRules[Random.Range(0, workingRules.Length)];
                bool applied = ApplyRule(chosenRule);
                Debug.Log(chosenRule);
            }

            Debug.Log($"{string.Join(";", mother.Vertices)} Count: {mother.Vertices.Count}");
        }

        public void ApplyUntilNoNonTerminal()
        {
            GrammarRule<TType>[] workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            while (workingRules.Any())
            {
                GrammarRule<TType> chosenRule = workingRules[Random.Range(0, workingRules.Count())];
                bool applied = ApplyRule(chosenRule);
                workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            }
        }
        
        private bool ApplyRule(GrammarRule<TType> rule)
        {
            List<Graph<TType>> subGraphs = mother.ContainedSubGraph(rule.LeftHandSide);

            Graph<TType> rightHandSide = rule.RightHandSide.Clone();
            
            if (subGraphs.Any())
            {
                //chose random subgraph to replace
                Graph<TType> subGraph = subGraphs[Random.Range(0, subGraphs.Count)];
                
                foreach (Vertex<TType> vertex in rightHandSide.Vertices)
                {
                    mother.AddVertex(vertex);
                }

                foreach (Vertex<TType> subGraphVertex in subGraph.Vertices)
                {
                    if (subGraphVertex != subGraph.End || subGraph.Start == subGraph.End)
                    {
                        //add edges to start
                        IEnumerable<Vertex<TType>> danglingOutGoingEdges = subGraphVertex.ForwardNeighbours.Except(subGraph.Vertices);
                        IEnumerable<Vertex<TType>> danglingIncomingEdges = subGraphVertex.IncomingNeighbours.Except(subGraph.Vertices);
                        foreach (Vertex<TType> danglingOutGoingEdge in danglingOutGoingEdges)
                        {
                            rightHandSide.Start.AddNextNeighbour(danglingOutGoingEdge);
                        }
                        foreach (Vertex<TType> danglingIncomingEdge in danglingIncomingEdges)
                        {
                            rightHandSide.Start.AddPreviousNeighbour(danglingIncomingEdge);
                        }
                    }
                    else
                    {
                        //add edges to end
                        IEnumerable<Vertex<TType>> danglingOutGoingEdges = subGraphVertex.ForwardNeighbours.Except(subGraph.Vertices);
                        IEnumerable<Vertex<TType>> danglingIncomingEdges = subGraphVertex.IncomingNeighbours.Except(subGraph.Vertices);
                        foreach (Vertex<TType> danglingOutGoingEdge in danglingOutGoingEdges)
                        {
                            rightHandSide.End.AddNextNeighbour(danglingOutGoingEdge);
                        }
                        foreach (Vertex<TType> danglingIncomingEdge in danglingIncomingEdges)
                        {
                            rightHandSide.End.AddPreviousNeighbour(danglingIncomingEdge);
                        }
                        
                    }
                }

                //change start if we replace start
                if (subGraph.Start == mother.Start)
                {
                    mother.Start = rightHandSide.Start;
                }

                //change end if we replace end
                if (subGraph.End == mother.End)
                {
                    mother.End = rightHandSide.End;
                }

                foreach (Vertex<TType> subGraphVertex in subGraph.Vertices)
                {
                    mother.RemoveVertex(subGraphVertex);
                }

                return true;
            }
            
            return false;
        }
    }
}