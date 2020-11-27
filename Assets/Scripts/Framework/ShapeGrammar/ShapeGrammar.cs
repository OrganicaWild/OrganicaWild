using System;
using System.Collections.Generic;
using System.Linq;
using Demo.ShapeGrammar;
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
                    tree.Root = new SpaceNode(
                        new MeshCorner() {connectionPoint = transform.position, connectionDirection = Vector3.zero},
                        ruleRep,
                        ruleRep.GetComponent<ShapeGrammarRuleComponent>(), missionVertex, new SpaceNode(gameObject));
                    tree.Root.InstantiatedReference = DrawNode(tree.Root);
                    tree.Leafs.Add(tree.Root);
                }
                else
                {
                    bool hasNoPlace = true;
                    MeshCorner openHook;
                    SpaceNode attachLeaf;
                    SpaceNode newNode;

                    var triedLeafs = new List<SpaceNode>();
                    //var leafsCopy = tree.Leafs.ToList();
                    do
                    {
                        var attachableLeafs = tree.Leafs.Except(triedLeafs).ToArray();
                        attachLeaf = attachableLeafs[Random.Range(0, attachableLeafs.Length)];
                        openHook = attachLeaf.GetOpenHook();

                        var rule = ruleRep.GetComponent<ShapeGrammarRuleComponent>();
                        newNode = new SpaceNode(openHook, ruleRep,
                            rule, missionVertex, attachLeaf);

                        newNode.InstantiatedReference = DrawNode(newNode);
                        
                        var b = newNode.InstantiatedReference.GetComponent<ShapeGrammarRuleComponent>();
                        b.Modify();


                        //newNode.Instantiated.SetActive(false);


                        //check for space
                        Vector3 potentialPosition =
                            newNode.InstantiatedReference.transform.position;
                        //Debug.DrawRay(Vector3.zero, newNode.parent.Instantiated.transform.position, Color.green, 1000f);
                        //Debug.DrawRay(Vector3.zero, potentialPosition, Color.green, 1000f);
                        // Debug.DrawLine(newNode.parent.Instantiated.transform.position, newNode.GetHook(), Color.blue, 1000f);
                        // Debug.DrawLine(newNode.GetEntryHook(), potentialPosition, Color.red, 1000f);
                        //Debug.Log($"{attachLeaf.Instantiated.transform.position}");

                        bool hitSomething = false;

                        foreach (MeshCorner hook in newNode.GetOpenHooks())
                        {
                            var worldHook = newNode.InstantiatedReference.transform.TransformPoint(hook.connectionPoint);
                            var worldDirection =
                                newNode.InstantiatedReference.transform.TransformDirection(hook.connectionDirection);
                            worldDirection *= 5;
                            hitSomething = Physics.Raycast(worldHook, worldDirection);
                            if (hitSomething)
                            {
                                Debug.DrawRay(worldHook, worldDirection, Color.red, 1000f);
                                break;
                            }
                        }

                        //if not space return to leaf and take other
                        if (!hitSomething)
                        {
                            //newNode.Instantiated.SetActive(true);
                            attachLeaf.RemoveOpenHook(openHook);
                            if (attachLeaf.GetNumberOfOpenHooks() == 0)
                            {
                                tree.Leafs.Remove(attachLeaf);
                            }

                            hasNoPlace = false;
                        }
                        else
                        {
                            triedLeafs.Add(attachLeaf);
                            //leafsCopy.Remove(attachLeaf);
                            Destroy(newNode.InstantiatedReference);
                        }
                    } while (hasNoPlace && triedLeafs.Count < tree.Leafs.Count);

                    if (triedLeafs.Count > tree.Leafs.Count)
                    {
                        Debug.LogError($"Ran out of spaces when trying to place {missionVertex.Type}");
                    }

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

        private static GameObject DrawNode(SpaceNode node)
        {
            Quaternion localRotation = node.GetLocalRotation();

            GameObject worldPiece =
                Instantiate(node.GetPrefab(), Vector3.zero, Quaternion.identity, node.parentSpaceNode.InstantiatedReference.transform);
            worldPiece.transform.localRotation = localRotation;


            worldPiece.transform.localPosition = node.GetLocalPosition();
            return worldPiece;
        }
    }
}