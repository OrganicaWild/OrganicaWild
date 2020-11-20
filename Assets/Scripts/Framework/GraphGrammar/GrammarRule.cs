using System.Collections;
using System.Collections.Generic;

namespace Framework.GraphGrammar
{
    public class GrammarRule
    {
        private IList<int> leftHandSide;
        private IList<int> rightHandSide;

        public GrammarRule(IList<int> leftHandSide, IList<int> rightHandSide)
        {
            this.leftHandSide = leftHandSide;
            this.rightHandSide = rightHandSide;
        }

        public bool FitLeftHandSide(Vertex vertex)
        {
            if (vertex.Type != leftHandSide[0])
                return false;

            Vertex current = vertex;
            for (int i = 1; i < leftHandSide.Count; i++)
            {
                int currentLeftHandSide = leftHandSide[i];
                foreach (Vertex newCurrent in current.ForwardNeighbours)
                {
                    if (newCurrent.Type != currentLeftHandSide)
                        return false;

                    current = newCurrent;
                }
            }

            return true;
        }
    }
}