using System;
using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar.Data;
using Framework.Util;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.GraphGrammar
{
    public class GraphGrammarComponent : MonoBehaviour
    {
        public List<EditorGrammarRule> rules = new List<EditorGrammarRule>();
        public EditorMissionGraph mother;
        private List<GrammarRule> internalRules;

        private MissionGraph internalMother;

        public GraphGrammar grammar;

        private IEnumerable<DrawableMissionVertex.ListElement> positions =
            new List<DrawableMissionVertex.ListElement>();

        private readonly Dictionary<MissionVertex, Vector3> dictionary = new Dictionary<MissionVertex, Vector3>(
            new IdentityEqualityComparer<MissionVertex>());

        public void MakeGrammar()
        {
            internalRules = new List<GrammarRule>();
            foreach (GrammarRule rule in rules.Select(editorGrammarRule => editorGrammarRule.GetGrammarRule()))
            {
                internalRules.Add(rule);
            }

            internalMother = mother.DeserializeAndConvert();
            
            grammar = new GraphGrammar(internalRules, internalMother);
            Debug.Log($"Replaced: {string.Join("; ", internalMother.Vertices)}");

            DrawableMissionVertex motherAsDrawable = internalMother.Start as DrawableMissionVertex;
            //show in unity
            positions =
                motherAsDrawable.Paint(new Vector3(0, 0, 0),
                    new List<DrawableMissionVertex.ListElement>(),
                    dictionary);
        }

        public void RunOneRule()
        {
            grammar.ApplyOneRule();
            positions =
                (internalMother.Start as DrawableMissionVertex).Paint(new Vector3(0, 0, 0),
                    new List<DrawableMissionVertex.ListElement>(),
                    dictionary);
            Debug.Log($"Replaced: {string.Join("; ", internalMother.Vertices)}");
        }

        public void ApplyUntilFinished()
        {
            grammar.ApplyUntilNoNonTerminal();
            positions =
                (internalMother.Start as DrawableMissionVertex).Paint(new Vector3(0, 0, 0),
                    new List<DrawableMissionVertex.ListElement>(),
                    dictionary);
            Debug.Log($"Replaced: {string.Join("; ", internalMother.Vertices)}");
        }

        private void OnDrawGizmos()
        {
            Dictionary<string, Color> colors = new Dictionary<string, Color>();

            Dictionary<MissionVertex, Vector3> dict =
                new Dictionary<MissionVertex, Vector3>(
                    new IdentityEqualityComparer<MissionVertex>());
            foreach (DrawableMissionVertex.ListElement position in positions)
            {
                Color color;
                if (colors.ContainsKey(position.t.Type))
                {
                    color = colors[position.t.Type];
                }
                else
                {
                    color = new Color(Random.value, Random.value, Random.value);
                    colors.Add(position.t.Type, color);
                }

                if (dict.ContainsKey(position.t))
                {
                    Gizmos.DrawLine(position.parent, dict[position.t]);
                }
                else
                {
                    Gizmos.color = color;
                    Gizmos.DrawCube(position.tPosition, new Vector3(0.1f, 0.1f, 0.1f));

                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(position.parent, position.tPosition);

                    GUIStyle guiStyle = new GUIStyle {fontSize = 30, fontStyle = FontStyle.Bold};

                    Handles.Label(position.tPosition, $"{position.t.Type}", guiStyle);
                    dict.Add(position.t, position.tPosition);
                }
            }
        }

        private void BuildRules()
        {
            //rule 00
            CreateLinearRule(new List<string>()
                {
                    "Start"
                },
                new List<string>()
                {
                    "Entrance", "Chain", "Gate",
                    "BossMini",
                    "ItemQuest", "TestItem",
                    "ChainFinal", "Goal"
                });

            //rule non linear 00
            MissionGraph left01 = CreateLinearGraph(new List<string>()
                {"ChainFinal", "Goal"});

            MissionGraph right01 = new MissionGraph();
            DrawableMissionVertex c = new DrawableMissionVertex(("Chain"));
            DrawableMissionVertex h0 = new DrawableMissionVertex(("Hook"));
            DrawableMissionVertex ga = new DrawableMissionVertex(("Gate"));
            DrawableMissionVertex lf = new DrawableMissionVertex(("LockFinal"));
            DrawableMissionVertex bl = new DrawableMissionVertex(("BossLevel"));
            DrawableMissionVertex go = new DrawableMissionVertex(("Goal"));
            DrawableMissionVertex t = new DrawableMissionVertex(("Test"));
            DrawableMissionVertex kf = new DrawableMissionVertex(("KeyFinal"));
            DrawableMissionVertex h1 = new DrawableMissionVertex(("Hook"));
            c.AddNextNeighbour(h0);
            c.AddNextNeighbour(t);
            c.AddNextNeighbour(ga);
            ga.AddNextNeighbour(lf);
            lf.AddNextNeighbour(bl);
            bl.AddNextNeighbour(go);
            t.AddNextNeighbour(kf);
            kf.AddNextNeighbour(lf);
            t.AddNextNeighbour(h1);
            right01.AddVertex(c);
            right01.AddVertex(h0);
            right01.AddVertex(ga);
            right01.AddVertex(lf);
            right01.AddVertex(bl);
            right01.AddVertex(go);
            right01.AddVertex(t);
            right01.AddVertex(kf);
            right01.AddVertex(h1);
            right01.Start = c;
            right01.End = go;
            GrammarRule rule = new GrammarRule(left01, right01);
            internalRules.Add(rule);

            //rule 01
            CreateLinearRule(new List<string>()
                {
                    "Chain", "Gate"
                },
                new List<string>()
                {
                    "ChainLinear", "ChainLinear", "ChainLinear",
                });

            //rule 02
            CreateLinearRule(new List<string>()
                {
                    "Chain", "Goal"
                },
                new List<string>()
                {
                    "ChainLinear", "ChainLinear", "ChainLinear",
                    "ChainLinear"
                });

            //rule 03
            CreateLinearRule(new List<string>()
                {
                    "Chain", "Goal"
                },
                new List<string>()
                {
                    "ChainLinear", "ChainLinear", "ChainLinear",
                    "ChainLinear",
                    "ChainLinear"
                });

            //rule 04
            CreateLinearRule(new List<string>()
                {
                    "ChainLinear"
                },
                new List<string>()
                {
                    "Test"
                });

            //rule 05
            CreateLinearRule(new List<string>()
                {
                    "ChainLinear"
                },
                new List<string>()
                {
                    "Test", "Test", "ItemBonus"
                });

            //rule 06
            CreateLinearRule(new List<string>()
                {
                    "ChainLinear"
                },
                new List<string>()
                {
                    "TestSecret"
                });

            //rule 07
            CreateLinearRule(
                new List<string>()
                {
                    "ChainLinear", "ChainLinear"
                },
                new List<string>()
                {
                    "Key", "Lock"
                });

            //rule 08
            CreateLinearRule(
                new List<string>()
                {
                    "ChainLinear", "ChainLinear"
                },
                new List<string>()
                {
                    "Key", "Lock", "ChainLinear"
                });

            //rule 09
            CreateLinearRule(new List<string>()
                {
                    "Chain", "Gate"
                },
                new List<string>()
                {
                    "ChainParallel", "Gate"
                });

            //rule 10 
            CreateLinearRule(
                new List<string>()
                {
                    "Fork", "KeyMultiPiece"
                },
                new List<string>()
                {
                    "Fork", "Test", "KeyMultiPiece"
                });

            //rule 11
            CreateLinearRule(
                new List<string>()
                {
                    "Fork", "KeyMultiPiece"
                },
                new List<string>()
                {
                    "Fork", "TestSecret", "KeyMultiPiece"
                });

            //rule 12
            CreateLinearRule(new List<string>()
                {
                    "Fork", "Key"
                },
                new List<string>()
                {
                    "Fork", "Test", "Key"
                });

            //rule 13
            CreateLinearRule(new List<string>()
                {
                    "Fork", "Key"
                },
                new List<string>()
                {
                    "Fork", "TestSecret", "Key"
                });

            //hook resolve 01
            CreateLinearRule(new List<string>()
                {
                    "Hook"
                },
                new List<string>()
                {
                    "Nothing"
                });

            //hook resolve 02
            CreateLinearRule(new List<string>()
                {
                    "Hook"
                },
                new List<string>()
                {
                    "Test", "ItemBonus"
                });

            //hook resolve 03
            CreateLinearRule(new List<string>()
                {
                    "Hook"
                },
                new List<string>()
                {
                    "TestSecret", "ItemBonus"
                });

            //rule non-linear 01
            MissionGraph left = CreateLinearGraph(new List<string>()
                {"ChainParallel", "Gate"});

            MissionGraph right = new MissionGraph();
            DrawableMissionVertex f = new DrawableMissionVertex(("Fork"));
            DrawableMissionVertex km0 = new DrawableMissionVertex(("KeyMultiPiece"));
            DrawableMissionVertex km1 = new DrawableMissionVertex(("KeyMultiPiece"));
            DrawableMissionVertex km2 = new DrawableMissionVertex(("KeyMultiPiece"));
            DrawableMissionVertex lm = new DrawableMissionVertex(("LockMulti"));
            f.AddNextNeighbour(km0);
            f.AddNextNeighbour(km1);
            f.AddNextNeighbour(km2);
            km0.AddNextNeighbour(lm);
            km1.AddNextNeighbour(lm);
            km2.AddNextNeighbour(lm);
            right.AddVertex(f);
            right.AddVertex(km0);
            right.AddVertex(km1);
            right.AddVertex(km2);
            right.AddVertex(lm);
            right.Start = f;
            right.End = lm;
            GrammarRule rule01 = new GrammarRule(left, right);
            internalRules.Add(rule01);

            //rule non-linear 02
            MissionGraph left02 = CreateLinearGraph(new List<string>()
                {"Fork", "KeyMultiPiece"});

            MissionGraph right02 = new MissionGraph();
            DrawableMissionVertex f0 = new DrawableMissionVertex(("Fork"));
            DrawableMissionVertex k = new DrawableMissionVertex(("Key"));
            DrawableMissionVertex l = new DrawableMissionVertex(("Lock"));
            DrawableMissionVertex km3 = new DrawableMissionVertex(("KeyMultiPiece"));
            DrawableMissionVertex h = new DrawableMissionVertex(("Hook"));
            f0.AddNextNeighbour(k);
            k.AddNextNeighbour(l);
            l.AddNextNeighbour(km3);
            l.AddNextNeighbour(h);
            right02.AddVertex(f0);
            right02.AddVertex(k);
            right02.AddVertex(l);
            right02.AddVertex(km3);
            right02.AddVertex(h);
            right02.Start = f0;
            right02.End = h;
            GrammarRule rule02 = new GrammarRule(left02, right02);
            internalRules.Add(rule02);

            //rule non-linear 03
            MissionGraph left03 = CreateLinearGraph(new List<string>() {"Fork"});
            MissionGraph right03 = new MissionGraph();
            DrawableMissionVertex n = new DrawableMissionVertex(("Nothing"));
            DrawableMissionVertex h02 = new DrawableMissionVertex(("Hook"));
            DrawableMissionVertex h03 = new DrawableMissionVertex(("Hook"));
            n.AddNextNeighbour(h02);
            n.AddNextNeighbour(h03);
            right03.AddVertex(n);
            right03.AddVertex(h02);
            right03.AddVertex(h03);
            right03.Start = n;
            right03.End = h02;
            GrammarRule rule03 = new GrammarRule(left03, right03);
            internalRules.Add(rule03);
        }

        private void CreateLinearRule(IList<string> leftHandSide, IList<string> rightHandSide)
        {
            MissionGraph left = CreateLinearGraph(leftHandSide);
            MissionGraph right = CreateLinearGraph(rightHandSide);
            GrammarRule rule = new GrammarRule(left, right);
            internalRules.Add(rule);
        }

        private MissionGraph CreateLinearGraph(IList<string> nodes)
        {
            MissionGraph missionGraph = new MissionGraph();

            DrawableMissionVertex prev = null;
            for (int index = 0;
                index < nodes.Count;
                index++)
            {
                string type = nodes[index];
                DrawableMissionVertex next = new DrawableMissionVertex(type);
                missionGraph.AddVertex(next);
                if (index == 0)
                {
                    missionGraph.Start = next;
                }

                if (index == nodes.Count - 1)
                {
                    missionGraph.End = next;
                }

                prev?.AddNextNeighbour(next);
                prev = next;
            }

            return missionGraph;
        }

        private void Save(MissionGraph graph, int n)
        {
            EditorMissionGraph missionGraph = ScriptableObject.CreateInstance<EditorMissionGraph>();
            AssetDatabase.CreateAsset(missionGraph,
                $"Assets/MissionGraph_{DateTime.Now:yy_MMM_dd_hh_mm_ss_ms}_{n}.asset");
            SerializedObject unitySerializedObject = new SerializedObject(missionGraph);
            SerializableMissionGraph serializableGraph = new SerializableMissionGraph(graph);
            string serialized = serializableGraph.Serialize();
            string _ = unitySerializedObject.FindProperty("serializedMissionGraph").stringValue = serialized;
            unitySerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(missionGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}