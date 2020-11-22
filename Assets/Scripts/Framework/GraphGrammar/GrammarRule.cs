using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.GraphGrammar
{
    public class GrammarRule
    {
        public Graph LeftHandSide { get; }
        public Graph RightHandSide { get; }

        public GrammarRule(Graph leftHandSide, Graph rightHandSide)
        {

            this.LeftHandSide = leftHandSide;
            this.RightHandSide = rightHandSide;
        }
    }
}