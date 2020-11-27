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
        internal SpaceNode Root { get; set; }
        internal IList<SpaceNode> Leafs { get; }

        public SpaceTree()
        {
            Leafs = new List<SpaceNode>();
        }
        
        
    }
}