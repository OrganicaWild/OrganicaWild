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
            // levelMissionGraph = new MissionGraph();
            // MissionVertex vertex = new MissionVertex("Entrance");
            // MissionVertex verte1x = new MissionVertex("Entrance");
            // vertex.AddNextNeighbour(verte1x);
            // levelMissionGraph.AddVertex(verte1x);
            // levelMissionGraph.AddVertex(vertex);
            // levelMissionGraph.Start = vertex;
            // levelMissionGraph.End = verte1x;

            BuildTree();
            //DrawTree();
        }

        private void BuildTree()
        {
            tree = new SpaceTree();
            List<MissionVertex> traversal = levelMissionGraph.Traverse();
            Debug.Log($"{string.Join(";", traversal)}");

            foreach (MissionVertex missionVertex in traversal)
            {
                List<GameObject> rulesForThisType = GetRulesForThisType(missionVertex);

                GameObject ruleRep = rulesForThisType[Random.Range(0, rulesForThisType.Count)];
                //AddSpaceNode(ruleRep, missionVertex);

                if (tree.Root == null)
                {
                    tree.Root = new SpaceNode(transform.position, ruleRep,
                        ruleRep.GetComponent<ShapeGrammarRuleComponent>(), missionVertex, new SpaceNode(gameObject));
                    tree.Root.Instantiated = DrawNode(tree.Root);
                    tree.Leafs.Add(tree.Root);
                }
                else
                {
                    bool hasNoPlace = true;
                    Vector3 openHook;
                    SpaceNode attachLeaf;
                    SpaceNode newNode;

                    do
                    {
                        attachLeaf = tree.Leafs[Random.Range(0, tree.Leafs.Count)];
                        openHook = attachLeaf.GetOpenHook();

                        newNode = new SpaceNode(openHook, ruleRep,
                            ruleRep.GetComponent<ShapeGrammarRuleComponent>(), missionVertex, attachLeaf);

                        newNode.Instantiated = DrawNode(newNode);
                        newNode.Instantiated.SetActive(false);

                        //check for space
                        Vector3 potentialPosition =
                            newNode.Instantiated.transform.position;
                        //Debug.DrawRay(Vector3.zero, newNode.parent.Instantiated.transform.position, Color.green, 1000f);
                        //Debug.DrawRay(Vector3.zero, potentialPosition, Color.green, 1000f);
                        // Debug.DrawLine(newNode.parent.Instantiated.transform.position, newNode.GetHook(), Color.blue, 1000f);
                        // Debug.DrawLine(newNode.GetEntryHook(), potentialPosition, Color.red, 1000f);
                        Debug.Log($"{attachLeaf.Instantiated.transform.position}");

                        bool hitSomething = false;
                        foreach (Vector3 hook in newNode.GetRotatedOpenHooks())
                        {
                            var worldHook = newNode.Instantiated.transform.TransformPoint(hook);
                            var scaledWorldHook = worldHook ;
                            hitSomething = Physics.Raycast(potentialPosition, scaledWorldHook);
                            if (hitSomething)
                            {
                                Debug.DrawRay(potentialPosition, scaledWorldHook, Color.red, 1000f);
                            }
                        }

                        //if not space return to leaf and take other
                        if (!hitSomething)
                        {
                            newNode.Instantiated.SetActive(true);
                            attachLeaf.RemoveOpenHook(openHook);
                            if (attachLeaf.GetNumberOfOpenHooks() == 0)
                            {
                                tree.Leafs.Remove(attachLeaf);
                            }

                            hasNoPlace = false;
                        }
                        else
                        {
                            Destroy(newNode.Instantiated);
                        }
                    } while (hasNoPlace);

                    if (newNode.GetNumberOfOpenHooks() > 0)
                    {
                        tree.Leafs.Add(newNode);
                    }
                }
            }
        }

        private List<GameObject> GetRulesForThisType(MissionVertex missionVertex)
        {
            List<GameObject> rulesForThisType = rules.Where(rule =>
            {
                ShapeGrammarRuleComponent c = rule.GetComponent<ShapeGrammarRuleComponent>();
                return c.type.Contains(missionVertex.Type);
            }).ToList();
            if (!rulesForThisType.Any())
            {
                Debug.LogError($"There is no rule to replace {missionVertex.Type}");
            }

            return rulesForThisType;
        }

        public void AddSpaceNode(GameObject shapeGrammarRule, MissionVertex missionVertex)
        {
            ShapeGrammarRuleComponent shapeGrammarRuleComponent =
                shapeGrammarRule.GetComponent<ShapeGrammarRuleComponent>();
            if (shapeGrammarRuleComponent == null)
            {
                throw new ArgumentException("The given GameObject does not have a Component ShapeGrammarRuler");
            }

            if (tree.Root == null)
            {
                tree.Root = new SpaceNode(Vector3.zero, shapeGrammarRule, shapeGrammarRuleComponent, missionVertex,
                    null);


                tree.Leafs.Add(tree.Root);
            }
            else
            {
                SpaceNode leaf = tree.Leafs[Random.Range(0, tree.Leafs.Count)];
                Vector3 hook = leaf.GetOpenHook();

                if (leaf.GetNumberOfOpenHooks() == 0)
                {
                    tree.Leafs.Remove(leaf);
                }

                SpaceNode newLeaf =
                    new SpaceNode(hook, shapeGrammarRule, shapeGrammarRuleComponent, missionVertex, leaf);
                if (newLeaf.GetNumberOfOpenHooks() > 0)
                {
                    tree.Leafs.Add(newLeaf);
                }

                leaf.AddBranch(newLeaf);
            }
        }

        public void FindNewPlaceForNode(SpaceNode node)
        {
            SpaceNode leaf;
            do
            {
                leaf = tree.Leafs[Random.Range(0, tree.Leafs.Count)];
            } while (leaf == node);

            Vector3 hook = leaf.GetOpenHook();

            if (leaf.GetNumberOfOpenHooks() == 0)
            {
                tree.Leafs.Remove(leaf);
            }
        }

        private void DrawTree()
        {
            SpaceNode root = tree.Root;
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
                GameObject worldPiece = DrawNode(v);

                visited.Add(v);

                foreach (SpaceNode node in v.branches)
                {
                    parentsGameObject.Add(node, worldPiece);
                    q.Enqueue(node);
                }
            }
        }

        private static GameObject DrawNode(SpaceNode node)
        {
            Quaternion localRotation = node.GetLocalRotation();

            GameObject worldPiece =
                Instantiate(node.GetPrefab(), Vector3.zero, Quaternion.identity, node.parent.Instantiated.transform);
            worldPiece.transform.localRotation = localRotation;

            worldPiece.transform.localPosition = node.GetLocalPosition();
            return worldPiece;
        }
    }
}