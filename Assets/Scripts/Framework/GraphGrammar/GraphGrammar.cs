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
        private readonly MissionGraph mother;

        public GraphGrammar(IList<GrammarRule> rules, MissionGraph mother)
        {
            this.rules = rules;
            this.mother = mother;
        }

        public MissionGraph GetLevel()
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
            List<MissionGraph> subGraphs = mother.ContainedSubGraph(rule.LeftHandSide);

            MissionGraph rightHandSide = rule.RightHandSide.Clone();
            
            if (subGraphs.Any())
            {
                //chose random subgraph to replace
                MissionGraph subMissionGraph = subGraphs[Random.Range(0, subGraphs.Count)];
                
                foreach (MissionVertex vertex in rightHandSide.Vertices)
                {
                    mother.AddVertex(vertex);
                }

                foreach (MissionVertex subGraphVertex in subMissionGraph.Vertices)
                {
                    if (subGraphVertex != subMissionGraph.End || subMissionGraph.Start == subMissionGraph.End)
                    {
                        //add edges to start
                        IEnumerable<MissionVertex> danglingOutGoingEdges = subGraphVertex.ForwardNeighbours.Except(subMissionGraph.Vertices);
                        IEnumerable<MissionVertex> danglingIncomingEdges = subGraphVertex.IncomingNeighbours.Except(subMissionGraph.Vertices);
                        foreach (MissionVertex danglingOutGoingEdge in danglingOutGoingEdges)
                        {
                            rightHandSide.Start.AddNextNeighbour(danglingOutGoingEdge);
                        }
                        foreach (MissionVertex danglingIncomingEdge in danglingIncomingEdges)
                        {
                            rightHandSide.Start.AddPreviousNeighbour(danglingIncomingEdge);
                        }
                    }
                    else
                    {
                        //add edges to end
                        IEnumerable<MissionVertex> danglingOutGoingEdges = subGraphVertex.ForwardNeighbours.Except(subMissionGraph.Vertices);
                        IEnumerable<MissionVertex> danglingIncomingEdges = subGraphVertex.IncomingNeighbours.Except(subMissionGraph.Vertices);
                        foreach (MissionVertex danglingOutGoingEdge in danglingOutGoingEdges)
                        {
                            rightHandSide.End.AddNextNeighbour(danglingOutGoingEdge);
                        }
                        foreach (MissionVertex danglingIncomingEdge in danglingIncomingEdges)
                        {
                            rightHandSide.End.AddPreviousNeighbour(danglingIncomingEdge);
                        }
                        
                    }
                }

                //change start if we replace start
                if (subMissionGraph.Start == mother.Start)
                {
                    mother.Start = rightHandSide.Start;
                }

                //change end if we replace end
                if (subMissionGraph.End == mother.End)
                {
                    mother.End = rightHandSide.End;
                }

                foreach (MissionVertex subGraphVertex in subMissionGraph.Vertices)
                {
                    mother.RemoveVertex(subGraphVertex);
                }

                return true;
            }
            
            return false;
        }
    }
}