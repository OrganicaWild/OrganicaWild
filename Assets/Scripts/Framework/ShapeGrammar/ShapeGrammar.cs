using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Framework.GraphGrammar;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Framework.ShapeGrammar
{
    public class ShapeGrammar : MonoBehaviour
    {
        /// <summary>
        /// Supply this in the editor with the prefabs of the shape grammar rules.
        /// The minimum shape grammar rule is an empty GameObject with a ShapeGrammarRuleComponent,
        /// a Rigidbody set to kinematic and a collider encompassing the full rule.
        /// </summary>
        public GameObject[] rules;

        private MissionGraph levelMissionGraph;
        private SpaceTree tree;

        private int numberOfNodes = 10000;
        private int steps = 100;

        public void Awake()
        {
            // GenerateLevel();
            // GenerateGeometry();

            StartCoroutine(nameof(Run));
        }

        private IEnumerator Run()
        {
            string path = @"C:\Users\Christoph\Desktop\results.txt";
            string timeString = "";
            string memoryString = "";
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i <= numberOfNodes; i += steps)
                {
                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }

                    yield return null;
                    
                    Stopwatch start = new Stopwatch();
                    start.Start();
                    long preInitiMemory = GC.GetTotalMemory(true);

                    GenerateLevel(i);
                    GenerateGeometry();

                    start.Stop();
                    timeString += $"{start.ElapsedMilliseconds} ms elapsed for {i} generations. \n";

                    long postInit = GC.GetTotalMemory(true);
                    memoryString +=
                        $"{postInit - preInitiMemory}        , {postInit} {preInitiMemory}  bytes allocated for {i} generations. \n";

                    Debug.Log($"max: {i} done");

                    if (i == numberOfNodes)
                    {
                        sw.Write(timeString);
                        sw.Write(memoryString);
                        sw.Flush();
                        Application.Quit();
                    }

                    yield return null;
                }
            }
        }

        /// <summary>
        /// Run the passed GraphGrammar to generate the underlying mission
        /// </summary>
        public void GenerateLevel(int max)
        {
            // GraphGrammarComponent graphGrammarComponent = GetComponent<GraphGrammarComponent>();
            // graphGrammarComponent.Initialize();
            // graphGrammarComponent.ApplyUntilNoRulesFitAnymore(max);
            //
            // levelMissionGraph = graphGrammarComponent.GetLevel();

            levelMissionGraph = new MissionGraph();
            levelMissionGraph.Start = new MissionVertex("Test");
            var prev = levelMissionGraph.Start;
            for (int i = 0; i < max; i++)
            {
                var newNode = new MissionVertex("Test");
                newNode.AddPreviousNeighbour(prev);
                prev = newNode;
            }

            levelMissionGraph.End = prev;
        }

        /// <summary>
        /// Clears all children of this GameObject
        /// </summary>
        public void ClearOldLevel()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Place the level down in geometry
        /// </summary>
        public void GenerateGeometry()
        {
            BuildTree();
        }

        private void BuildTree()
        {
            tree = new SpaceTree();
            List<MissionVertex> traversal = levelMissionGraph.Traverse();
            Debug.Log($" {traversal.Count}");

            foreach (MissionVertex missionVertex in traversal)
            {
                List<GameObject> rulesForThisType = GetRulesForThisType(missionVertex);

                GameObject ruleRep = rulesForThisType[Random.Range(0, rulesForThisType.Count)];
                //AddSpaceNode(ruleRep, missionVertex);

                if (tree.Root == null)
                {
                    tree.Root = new SpaceNode(
                        new SpaceNodeConnection()
                            {connectionPoint = transform.position, connectionDirection = Vector3.zero},
                        ruleRep,
                        ruleRep.GetComponent<ShapeGrammarRuleComponent>(), missionVertex, new SpaceNode(gameObject));
                    tree.Root.InstantiatedReference = DrawNode(tree.Root);
                    tree.Leafs.Add(tree.Root);
                }
                else
                {
                    bool hasNoPlace = true;
                    SpaceNodeConnection openHook;
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

                        foreach (SpaceNodeConnection hook in newNode.GetOpenHooks())
                        {
                            var worldHook =
                                newNode.InstantiatedReference.transform.TransformPoint(hook.connectionPoint);
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

                    if (triedLeafs.Count >= tree.Leafs.Count)
                    {
                        Debug.LogWarning($"Ran out of spaces when trying to place {missionVertex.Type}");
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
                Instantiate(node.GetPrefab(), Vector3.zero, Quaternion.identity,
                    node.parentSpaceNode.InstantiatedReference.transform);
            worldPiece.transform.localRotation = localRotation;


            worldPiece.transform.localPosition = node.GetLocalPosition();
            return worldPiece;
        }
    }
}