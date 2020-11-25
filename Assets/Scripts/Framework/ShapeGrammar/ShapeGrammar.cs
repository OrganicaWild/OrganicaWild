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
            Debug.Log("tree built");
            Debug.Log($"{Quaternion.identity}");
            DrawTree();
        }

        private void BuildTree()
        {
            tree = new SpaceTree();
            List<Vertex<DDorman>> traversal = levelGraph.Traverse();
            Debug.Log($"{string.Join(";", traversal)}");

            for (int index = 0; index < 20; index++)
            {
                //Vertex<DDorman> vertex = traversal[index];
                GameObject ruleRep = rules[Random.Range(0, rules.Length)];
                tree.AddSpaceNode(ruleRep);
            }
        }

        private void DrawTree()
        {
            SpaceNode root = tree.GetRoot();
            Queue<SpaceNode> q = new Queue<SpaceNode>();

            q.Enqueue(root);
            HashSet<SpaceNode> visited = new HashSet<SpaceNode>(new IdentityEqualityComparer<SpaceNode>());
            Dictionary<SpaceNode, Vector3> positions =
                new Dictionary<SpaceNode, Vector3>(new IdentityEqualityComparer<SpaceNode>());

            Dictionary<SpaceNode, Quaternion> rotations =
                new Dictionary<SpaceNode, Quaternion>(new IdentityEqualityComparer<SpaceNode>());

            Dictionary<SpaceNode, SpaceNode> parents =
                new Dictionary<SpaceNode, SpaceNode>(new IdentityEqualityComparer<SpaceNode>());

            positions.Add(root, -root.GetEntryHook());
            rotations.Add(root, Quaternion.identity);

            GameObject parent = null;

            while (q.Any())
            {
                SpaceNode v = q.Dequeue();
                if (visited.Contains(v))
                {
                    continue;
                }

                //draw actual tree
                Vector3 localPosition = positions[v];


                if (parent != null)
                {
                    var a = v.GetEntryHook();
                    var b = -v.GetHook();

                    Debug.DrawRay(Vector3.zero, a, Color.red, 1000);
                    Debug.DrawRay(Vector3.zero, b, Color.blue, 1000);
                    Debug.Log($"{b}");


                    var cross = Vector3.Cross(a, b);

                    var sign = Mathf.Sign(cross.y);

                    var dot = Vector3.Dot(a, b);
                    float newrotation = sign * Mathf.Acos(dot);
                    var localRotation = Quaternion.Euler(0, newrotation * 180 / Mathf.PI, 0);
                    Debug.Log($"cross {cross} x, dot {dot},,,{newrotation} {localRotation}");

                    var rotatedA = localRotation * v.GetEntryHook();
                    Debug.DrawRay(Vector3.zero, rotatedA, Color.green, 1000);

                    GameObject worldPiece =
                        Instantiate(v.GetPrefab(), Vector3.zero, Quaternion.identity, parent.transform);
                    worldPiece.transform.localRotation = localRotation;
                    v.RotateHooks(localRotation);

                    worldPiece.transform.localPosition = v.GetHook() - v.GetEntryHook();

                    parent = worldPiece;
                }
                else
                {
                    GameObject worldPiece = Instantiate(v.GetPrefab(), Vector3.zero, Quaternion.identity);
                    // worldPiece.transform.localRotation = localRotation;
                    // v.RotateHooks(localRotation);

                    worldPiece.transform.localPosition = v.GetEntryHook() - v.GetEntryHook();

                    parent = worldPiece;
                }

                visited.Add(v);

                foreach (SpaceNode node in v.branches)
                {
                    positions.Add(node, node.GetHook());
                    parents.Add(node, v);
                    //rotations.Add(node, localRotation);
                    q.Enqueue(node);
                }
            }
        }

        public static Quaternion ExtractRotation(Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }
    }
}