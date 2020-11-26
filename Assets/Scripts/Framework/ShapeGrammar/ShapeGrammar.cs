using System;
using System.Collections.Generic;
using System.Linq;
using Demo.GraphGrammar;
using Framework.GraphGrammar;
using Framework.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.ShapeGrammar
{
    public class ShapeGrammar : MonoBehaviour
    {
        public GameObject[] rules;
        private Graph<DDorman> levelGraph;
        private SpaceTree tree;
  
        public void Awake()
        {
            DormanGrammar graphGrammar = GetComponent<DormanGrammar>();
            graphGrammar.MakeGrammar();
            graphGrammar.ApplyUntilFinished();

            levelGraph = graphGrammar.grammar.GetLevel();
            BuildTree();
            DrawTree();
        }

        private void BuildTree()
        {
            tree = new SpaceTree();
            List<Vertex<DDorman>> traversal = levelGraph.Traverse();
            Debug.Log($"{string.Join(";", traversal)}");

            for (int index = 0; index < traversal.Count; index++)
            {
                Vertex<DDorman> vertex = traversal[index];
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
                    Vector3 a = v.GetEntryHook();
                    Vector3 b = -v.GetHook();

                    // Debug.DrawRay(Vector3.zero, a, Color.red, 1000);
                    // Debug.DrawRay(Vector3.zero, b, Color.blue, 1000);
                    // Debug.Log($"{b}");

                    Vector3 cross = Vector3.Cross(a, b);

                    float sign = Mathf.Sign(cross.y);

                    float dot = Vector3.Dot(a, b);
                    float newrotation = sign * Mathf.Acos(dot);
                    Quaternion localRotation = Quaternion.Euler(0, newrotation * 180 / Mathf.PI, 0);

                    Vector3 rotatedA = localRotation * v.GetEntryHook();
                    // Debug.DrawRay(Vector3.zero, rotatedA, Color.green, 1000);

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