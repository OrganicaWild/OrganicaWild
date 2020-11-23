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

        public int RuleCount()
        {
            return rules.Count;
        }

        public void ApplyOneRule()
        {
            var workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            if (workingRules.Any())
            {
                GrammarRule<TType> chosenRule = workingRules[Random.Range(0, workingRules.Length)];
                bool applied = ApplyRule(chosenRule);
                Debug.Log(applied);
            }
        }

        public void ApplyUntilNoNonTerminal()
        {
            // ApplyRule(rules[0]);
            // ApplyRule(rules[1]);
            // ApplyRule(rules[14]);
            GrammarRule<TType>[] workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            while (workingRules.Any())
            {
                GrammarRule<TType> chosenRule = workingRules[Random.Range(0, workingRules.Count())];
                bool applied = ApplyRule(chosenRule);
                workingRules = this.rules.Where(x => mother.Contains(x.LeftHandSide)).ToArray();
            }
        }

        private bool MotherHasNonTerminal()
        {
            IList<Vertex<TType>> traversal = mother.Dfs();
            bool hasNonTerminal = traversal.Any(vertex => !vertex.Type.IsTerminal());
            return hasNonTerminal;
        }

        public bool ApplyRule(GrammarRule<TType> rule)
        {
            IList<Vertex<TType>> thisDfs = mother.Dfs();
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

            chosenStart.TransferIncomingEdges(rightHandSideCopy.Start);
            chosenEnd.TransferOutgoingEdges(rightHandSideCopy.End);

            // //remove the rest of edges on each of these
            // if (chosenEnd != chosenStart)
            // {
            //     chosenEnd.RemoveIncomingEdges();
            //     chosenStart.RemoveOutgoingEdges();
            // }

            //if we replace start, we gotta switch the start reference
            if (chosenStart == mother.Start)
            {
                mother.Start = rightHandSideCopy.Start;
            }

            //if we replace end, we gotta switch the end reference
            if (chosenEnd == mother.End)
            {
                mother.End = rightHandSideCopy.End;
            }

            foreach (Vertex<TType> v in rightHandSideCopy.Dfs())
            {
                mother.AddVertex(v);
            }

            mother.RemoveVertex(chosenStart);
            mother.RemoveVertex(chosenEnd);

            return true;
        }
    }
}