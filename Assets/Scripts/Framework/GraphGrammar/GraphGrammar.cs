using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.GraphGrammar
{
    public class GraphGrammar
    {
        private readonly IList<GrammarRule> rules;
        private readonly Graph mother;

        public GraphGrammar(IList<GrammarRule> rules, Graph mother)
        {
            this.rules = rules;
            this.mother = mother;
        }

        public Graph GetLevel()
        {
            return mother.Clone();
        }

        public int RuleCount()
        {
            return rules.Count;
        }

        public void ApplyOneRule()
        {
            GrammarRule[] workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            if (workingRules.Any())
            {
                GrammarRule chosenRule = workingRules[Random.Range(0, workingRules.Length)];
                bool applied = ApplyRule(chosenRule);
                Debug.Log(chosenRule);
            }

            Debug.Log($"{string.Join(";", mother.Vertices)} Count: {mother.Vertices.Count}");
        }

        public void ApplyUntilNoNonTerminal()
        {
            GrammarRule[] workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            while (workingRules.Any())
            {
                GrammarRule chosenRule = workingRules[Random.Range(0, workingRules.Count())];
                bool applied = ApplyRule(chosenRule);
                workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            }
        }
        
        private bool ApplyRule(GrammarRule rule)
        {
            List<Graph> subGraphs = mother.ContainedSubGraph(rule.LeftHandSide);

            Graph rightHandSide = rule.RightHandSide.Clone();
            
            if (subGraphs.Any())
            {
                //chose random subgraph to replace
                Graph subGraph = subGraphs[Random.Range(0, subGraphs.Count)];
                
                foreach (Vertex vertex in rightHandSide.Vertices)
                {
                    mother.AddVertex(vertex);
                }

                foreach (Vertex subGraphVertex in subGraph.Vertices)
                {
                    if (subGraphVertex != subGraph.End || subGraph.Start == subGraph.End)
                    {
                        //add edges to start
                        IEnumerable<Vertex> danglingOutGoingEdges = subGraphVertex.ForwardNeighbours.Except(subGraph.Vertices);
                        IEnumerable<Vertex> danglingIncomingEdges = subGraphVertex.IncomingNeighbours.Except(subGraph.Vertices);
                        foreach (Vertex danglingOutGoingEdge in danglingOutGoingEdges)
                        {
                            rightHandSide.Start.AddNextNeighbour(danglingOutGoingEdge);
                        }
                        foreach (Vertex danglingIncomingEdge in danglingIncomingEdges)
                        {
                            rightHandSide.Start.AddPreviousNeighbour(danglingIncomingEdge);
                        }
                    }
                    else
                    {
                        //add edges to end
                        IEnumerable<Vertex> danglingOutGoingEdges = subGraphVertex.ForwardNeighbours.Except(subGraph.Vertices);
                        IEnumerable<Vertex> danglingIncomingEdges = subGraphVertex.IncomingNeighbours.Except(subGraph.Vertices);
                        foreach (Vertex danglingOutGoingEdge in danglingOutGoingEdges)
                        {
                            rightHandSide.End.AddNextNeighbour(danglingOutGoingEdge);
                        }
                        foreach (Vertex danglingIncomingEdge in danglingIncomingEdges)
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

                foreach (Vertex subGraphVertex in subGraph.Vertices)
                {
                    mother.RemoveVertex(subGraphVertex);
                }

                return true;
            }
            
            return false;
        }
    }
}