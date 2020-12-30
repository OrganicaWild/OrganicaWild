using System;

namespace Framework.GraphGrammar
{
    /// <summary>
    /// Provides Grammar Rules for the GraphGrammar.
    /// </summary>
    [Serializable]
    public class GrammarRule
    {
        /// <summary>
        /// When applying this rule. This graph is searched for in the mother graph and replaced with the RightHandSide.
        /// </summary>
        public MissionGraph LeftHandSide { get; }

        /// <summary>
        /// This graph is the replacement in the mother graph for the LeftHandSide, when this rule is applied.
        /// </summary>
        public MissionGraph RightHandSide { get; }

        /// <summary>
        /// Create a new GrammarRule with the supplied sides.
        /// </summary>
        /// <param name="leftHandSide">To replace subgraph</param>
        /// <param name="rightHandSide">Replacement subgraph</param>
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