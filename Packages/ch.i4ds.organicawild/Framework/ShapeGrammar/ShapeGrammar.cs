using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using UnityEngine;
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

        public void Awake()
        {
            GenerateLevel();
            GenerateGeometry();
        }

        /// <summary>
        /// Run the passed GraphGrammar to generate the underlying mission
        /// </summary>
        private void GenerateLevel()
        {
            GraphGrammarComponent graphGrammarComponent = GetComponent<GraphGrammarComponent>();
            graphGrammarComponent.Initialize();
            graphGrammarComponent.ApplyUntilNoRulesFitAnymore();

            levelMissionGraph = graphGrammarComponent.GetLevel();
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

            foreach (MissionVertex missionVertex in traversal)
            {
                List<GameObject> rulesForThisType = GetRulesForThisType(missionVertex);

                GameObject ruleRep = rulesForThisType[Random.Range(0, rulesForThisType.Count)];

                if (tree.Root == null)
                {
                    tree.Root = new SpaceNode(
                        new SpaceNodeConnection {connectionPoint = transform.position, connectionDirection = Vector3.zero},
                        ruleRep,
                        ruleRep.GetComponent<ShapeGrammarRuleComponent>(), missionVertex, new SpaceNode(gameObject));
                    tree.Root.InstantiatedReference = DrawNode(tree.Root);
                    tree.Leafs.Add(tree.Root);
                }
                else
                {
                    bool hasNoPlace = true;
                    SpaceNode newNode;

                    List<SpaceNode> triedLeafs = new List<SpaceNode>();
                    do
                    {
                        SpaceNode[] attachableLeafs = tree.Leafs.Except(triedLeafs).ToArray();
                        SpaceNode attachLeaf = attachableLeafs[Random.Range(0, attachableLeafs.Length)];
                        SpaceNodeConnection openHook = attachLeaf.GetOpenHook();

                        ShapeGrammarRuleComponent rule = ruleRep.GetComponent<ShapeGrammarRuleComponent>();
                        newNode = new SpaceNode(openHook, ruleRep,
                            rule, missionVertex, attachLeaf);

                        newNode.InstantiatedReference = DrawNode(newNode);

                        ShapeGrammarRuleComponent b = newNode.InstantiatedReference.GetComponent<ShapeGrammarRuleComponent>();
                        b.Modify();

                        bool hitSomething = false;

                        foreach (SpaceNodeConnection hook in newNode.GetOpenHooks())
                        {
                            Vector3 worldHook =
                                newNode.InstantiatedReference.transform.TransformPoint(hook.connectionPoint);
                            Vector3 worldDirection =
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
                Instantiate(node.GetPrefab(), Vector3.zero, Quaternion.identity,
                    node.parentSpaceNode.InstantiatedReference.transform);
            worldPiece.transform.localRotation = localRotation;


            worldPiece.transform.localPosition = node.GetLocalPosition();
            return worldPiece;
        }
    }
}