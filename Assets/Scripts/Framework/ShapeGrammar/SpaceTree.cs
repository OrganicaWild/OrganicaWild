using System;
using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.ShapeGrammar
{
    /// <summary>
    /// Provides a Utility for building a Tree with SpaceNodes
    /// </summary>
    internal class SpaceTree
    {
        internal SpaceNode Root { get; set; }
        internal IList<SpaceNode> Leafs { get; }

        internal SpaceTree()
        {
            Leafs = new List<SpaceNode>();
        }
    }
}