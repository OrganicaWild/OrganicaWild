using System;
using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using Framework.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.ShapeGrammar
{
    public class ShapeGrammar : MonoBehaviour
    {
        public GameObject[] rules;
        private Graph levelGraph;
        private SpaceTree tree;
  
        public void Awake()
        {
            GraphGrammarComponent graphGrammarComponent = GetComponent<GraphGrammarComponent>();
            graphGrammarComponent.MakeGrammar();
            graphGrammarComponent.ApplyUntilFinished();

            levelGraph = graphGrammarComponent.grammar.GetLevel();
            BuildTree();
            DrawTree();
        }

        private void BuildTree()
        {
            tree = new SpaceTree();
            List<Vertex> traversal = levelGraph.Traverse();
            Debug.Log($"{string.Join(";", traversal)}");

            for (int index = 0; index < traversal.Count; index++)
            {
                Vertex vertex = traversal[index];
                List<GameObject> rulesForThisType = rules.Where(rule =>
                {
                    ShapeGrammarRuler c = rule.GetComponent<ShapeGrammarRuler>();
                    return Equals(c.type, vertex.Type);
                }).ToList();
                if (rulesForThisType.Any())
                {
                    GameObject ruleRep = rulesForThisType[Random.Range(0, rulesForThisType.Count)];
                    tree.AddSpaceNode(ruleRep);
                }
                else
                {
                    Debug.LogError($"There is no rule to replace {vertex.Type}");
                }
            }
        }

        private void DrawTree()
        {
            SpaceNode root = tree.GetRoot();
            Queue<SpaceNode> q = new Queue<SpaceNode>();
            q.Enqueue(root);
            HashSet<SpaceNode> visited = new HashSet<SpaceNode>(new IdentityEqualityComparer<SpaceNode>());
            Dictionary<SpaceNode, GameObject> parentsGameObject =
                new Dictionary<SpaceNode, GameObject>(new IdentityEqualityComparer<SpaceNode>()) {{root, null}};

            while (q.Any())
            {
                SpaceNode v = q.Dequeue();
                if (visited.Contains(v))
                {
                    continue;
                }

                //draw actual tree
                GameObject parent = parentsGameObject[v];
                GameObject worldPiece;
                if (parent != null)
                {
                    Quaternion localRotation = v.GetLocalRotation();

                    worldPiece =
                        Instantiate(v.GetPrefab(), Vector3.zero, Quaternion.identity, parent.transform);
                    worldPiece.transform.localRotation = localRotation;
                    v.RotateHooks(localRotation);
                    worldPiece.transform.localPosition = v.GetHook() - v.GetEntryHook();
                }
                else
                {
                    worldPiece = Instantiate(v.GetPrefab(), Vector3.zero, Quaternion.identity);
                    worldPiece.transform.localPosition = v.GetEntryHook() - v.GetEntryHook();
                }

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