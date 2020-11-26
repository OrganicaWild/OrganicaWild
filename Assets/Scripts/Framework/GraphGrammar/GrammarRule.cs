using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.GraphGrammar
{
    public class GrammarRule<TType>
    {
        public Graph<TType> LeftHandSide { get; }
        public Graph<TType> RightHandSide { get; }

        public GrammarRule(Graph<TType> leftHandSide, Graph<TType> rightHandSide)
        {

            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;
        }

        public override string ToString()
        {
            return $"{LeftHandSide} -> {RightHandSide}";
        }
    }
}