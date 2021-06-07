using System.Collections.Generic;

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