using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.GraphGrammar
{
    [Serializable]
    public class GrammarRule
    {
        public MissionGraph LeftHandSide { get; }
        public MissionGraph RightHandSide { get; }

        public GrammarRule(MissionGraph leftHandSide, MissionGraph rightHandSide)
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