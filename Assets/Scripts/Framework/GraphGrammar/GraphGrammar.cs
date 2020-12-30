using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Framework.GraphGrammar
{
    /// <summary>
    /// This Class represents a Graph Grammar with all its components
    ///
    /// The class contains all of the rules of the graph grammar and the initial graph.
    /// The initial graph is 
    /// 
    /// </summary>
    public class GraphGrammar
    {
        private readonly IList<GrammarRule> rules;
        private readonly MissionGraph mother;

        /// <summary>
        /// Creates a Graph Grammar with the passed rules and the given mother graph.
        /// The mother graph is the graph that will turn into the level graph after applied rules.
        /// </summary>
        /// <param name="rules">List of grammar rules</param>
        /// <param name="mother">Initial graph for manipulation</param>
        public GraphGrammar(IList<GrammarRule> rules, MissionGraph mother)
        {
            this.rules = rules;
            this.mother = mother;
        }

        /// <summary>
        /// Returns a copy of the level graph in it's current state.
        /// </summary>
        public MissionGraph GetLevel()
        {
            return mother.Clone();
        }
        
       /// <summary>
       /// Applies one random rule from the supplied rules on the level graph.
       /// A rule can be applied several times. It is not removed after being applied once.
       /// </summary>
       /// <exception cref="Exception">thrown if no rules can be applied to the level graph</exception>
        public void ApplyOneRule()
        {
            GrammarRule[] workingRules = this.rules.Where(x => mother.ContainsSubGraphBool(x.LeftHandSide)).ToArray();
            if (workingRules.Any())
            {
                GrammarRule chosenRule = workingRules[Random.Range(0, workingRules.Length)];
                ApplyRule(chosenRule);
            }
            else
            {
                throw new Exception("No fitting rules found.");
            }
            
        }

       /// <summary>
       /// Applies rules to the level graph, until no rule can be applied anymore.
       /// </summary>
       /// <returns>number of applied rules</returns>
        public int ApplyUntilNoRulesFitAnymore()
        {
            GrammarRule[] workingRules = rules.Where(x => mother.ContainsSubGraphBool(x.LeftHandSide)).ToArray();
            int appliedRules = 0;
            while (workingRules.Any())
            {
                GrammarRule chosenRule = workingRules[Random.Range(0, workingRules.Count())];
                ApplyRule(chosenRule);
                workingRules = this.rules.Where(x => mother.ContainsSubGraphBool(x.LeftHandSide)).ToArray();
                appliedRules++;
            }

            return appliedRules;
        }
        
        private bool ApplyRule(GrammarRule rule)
        {
            List<MissionGraph> subGraphs = mother.ContainsSubGraphMultiple(rule.LeftHandSide);

            MissionGraph rightHandSide = rule.RightHandSide.Clone();
            
            if (subGraphs.Any())
            {
                //chose random subgraph to replace, if there are several fitting sub graphs
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