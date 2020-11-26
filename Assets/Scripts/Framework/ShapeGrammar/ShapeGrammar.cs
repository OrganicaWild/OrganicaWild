using System;
using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using Framework.Util;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.ShapeGrammar
{
    public class ShapeGrammar : MonoBehaviour
    {
        public GameObject[] rules;
        private MissionGraph levelMissionGraph;
        private SpaceTree tree;

        public void Awake()
        {
            GraphGrammarComponent graphGrammarComponent = GetComponent<GraphGrammarComponent>();
            graphGrammarComponent.MakeGrammar();
            graphGrammarComponent.ApplyUntilFinished();

            levelMissionGraph = graphGrammarComponent.grammar.GetLevel();
            BuildTree();
            DrawTree();
        }

        private void BuildTree()
        {
            tree = new SpaceTree();
            List<MissionVertex> traversal = levelMissionGraph.Traverse();
            Debug.Log($"{string.Join(";", traversal)}");

            for (int index = 0; index < traversal.Count; index++)
            {
                MissionVertex missionVertex = traversal[index];
                List<GameObject> rulesForThisType = rules.Where(rule =>
                {
                    ShapeGrammarRuleComponent c = rule.GetComponent<ShapeGrammarRuleComponent>();
                    return c.type.Contains(missionVertex.Type);
                }).ToList();
                if (rulesForThisType.Any())
                {
                    GameObject ruleRep = rulesForThisType[Random.Range(0, rulesForThisType.Count)];
                    AddSpaceNode(ruleRep, missionVertex);
                }
                else
                {
                    Debug.LogError($"There is no rule to replace {missionVertex.Type}");
                }
            }
        }

        public void AddSpaceNode(GameObject shapeGrammarRule, MissionVertex missionVertex)
        {
            ShapeGrammarRuleComponent shapeGrammarRuleComponent =
                shapeGrammarRule.GetComponent<ShapeGrammarRuleComponent>();
            if (shapeGrammarRuleComponent == null)
            {
                throw new ArgumentException("The given GameObject does not have a Component ShapeGrammarRuler");
            }

            if (tree.root == null)
            {
                tree.root = new SpaceNode(Vector3.zero, shapeGrammarRule, shapeGrammarRuleComponent, missionVertex, null);
                tree.leafs.Add(tree.root);
            }
            else
            {
                SpaceNode leaf = tree.leafs[Random.Range(0, tree.leafs.Count)];
                Vector3 hook = leaf.GetOpenHook();

                if (leaf.GetNumberOfOpenHooks() == 0)
                {
                    tree.leafs.Remove(leaf);
                }

                SpaceNode newLeaf = new SpaceNode(hook, shapeGrammarRule, shapeGrammarRuleComponent, missionVertex, leaf);
                if (newLeaf.GetNumberOfOpenHooks() > 0)
                {
                    tree.leafs.Add(newLeaf);
                }

                leaf.AddBranch(newLeaf);
            }
        }

        public void FindNewPlaceForNode(SpaceNode node)
        {
            SpaceNode leaf;
            do
            {
                leaf = tree.leafs[Random.Range(0, tree.leafs.Count)];
            } while (leaf == node);

            Vector3 hook = leaf.GetOpenHook();

            if (leaf.GetNumberOfOpenHooks() == 0)
            {
                tree.leafs.Remove(leaf);
            }
        }

        private void DrawTree()
        {
            SpaceNode root = tree.GetRoot();
            Queue<SpaceNode> q = new Queue<SpaceNode>();
            q.Enqueue(root);
            HashSet<SpaceNode> visited = new HashSet<SpaceNode>(new IdentityEqualityComparer<SpaceNode>());
            Dictionary<SpaceNode, GameObject> parentsGameObject =
                new Dictionary<SpaceNode, GameObject>(new IdentityEqualityComparer<SpaceNode>()) {{root, gameObject}};

            while (q.Any())
            {
                SpaceNode v = q.Dequeue();
                if (visited.Contains(v))
                {
                    continue;
                }

                GameObject parent = parentsGameObject[v];
                Quaternion localRotation = v.GetLocalRotation();

                GameObject worldPiece = Instantiate(v.GetPrefab(), Vector3.zero, Quaternion.identity, parent.transform);
                worldPiece.transform.localRotation = localRotation;
                v.RotateHooks(localRotation);

                worldPiece.transform.localPosition = v.GetHook() - v.GetEntryHook();

                visited.Add(v);

                foreach (SpaceNode node in v.branches)
                {
                    parentsGameObject.Add(node, worldPiece);
                    q.Enqueue(node);
                }
            }
        }
    }
}