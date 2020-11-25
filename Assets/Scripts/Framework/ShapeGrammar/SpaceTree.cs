using System;
using System.Collections.Generic;
using System.Linq;
using Demo.GraphGrammar;
using Framework.GraphGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.ShapeGrammar
{
    public class SpaceTree
    {
        private SpaceNode root;
        private IList<SpaceNode> leafs;

        public SpaceTree()
        {
            leafs = new List<SpaceNode>();
        }

        public SpaceNode GetRoot()
        {
            return root;
        }
        
        public void AddSpaceNode(GameObject shapeGrammarRule)
        {
            ShapeGrammarRuler shapeGrammarRuler = shapeGrammarRule.GetComponent<ShapeGrammarRuler>();
            if (shapeGrammarRuler == null)
            {
                throw new ArgumentException("The given GameObject does not have a Component ShapeGrammarRuler");
            }

            if (root == null)
            {
                root = new SpaceNode(Vector3.zero, shapeGrammarRule, shapeGrammarRuler);
                leafs.Add(root);
            }
            else
            {
                SpaceNode leaf = leafs[Random.Range(0, leafs.Count)];
                Vector3 hook = leaf.GetOpenHook();

                if (leaf.GetNumberOfOpenHooks() == 0)
                {
                    leafs.Remove(leaf);
                }
            
                SpaceNode newLeaf = new SpaceNode(hook, shapeGrammarRule, shapeGrammarRuler);
                if (newLeaf.GetNumberOfOpenHooks() > 0)
                {
                    leafs.Add(newLeaf);
                }
                leaf.AddBranch(newLeaf);
            }
            
        }
        
    }
}