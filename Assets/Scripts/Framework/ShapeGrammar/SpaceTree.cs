using System;
using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.ShapeGrammar
{
    public class SpaceTree
    {
        internal SpaceNode root;
        internal IList<SpaceNode> leafs;

        public SpaceTree()
        {
            leafs = new List<SpaceNode>();
        }

        public SpaceNode GetRoot()
        {
            return root;
        }

        
        
    }
}