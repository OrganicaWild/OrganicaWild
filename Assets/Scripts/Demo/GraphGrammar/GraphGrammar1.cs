using System;
using System.Collections.Generic;
using Framework.GraphGrammar;
using Framework.Util;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.GraphGrammar
{
    public class GraphGrammar1 : MonoBehaviour
    {
        public GameObject vertexRepresentation;

        private IList<GrammarRule<DDorman>> rules = new List<GrammarRule<DDorman>>();
        private Graph<DDorman> mother;

        public GraphGrammar<DDorman> grammar;

        private IEnumerable<DrawableVertex<DDorman>.ListElement> positions =
            new List<DrawableVertex<DDorman>.ListElement>();
        
        private Dictionary<Vertex<DDorman>, Vector3> dictionary = new Dictionary<Vertex<DDorman>, Vector3>(
            new IdentityEqualityComparer<Vertex<DDorman>>());

        private void Awake()
        {
            MakeGrammar();
            grammar.ApplyUntilNoNonTerminal();
        }

        public void MakeGrammar()
        {
            BuildRules();

            //mother graph
            mother = CreateLinearGraph(new List<DDorman.DDormanType>() {DDorman.DDormanType.Start});

            grammar = new GraphGrammar<DDorman>(rules, mother);
            
            //apply rule
            // bool replaced = grammar.ApplyRule(0);
            // replaced = grammar.ApplyRule(1);

            Debug.Log($"Replaced: {string.Join("; ", mother.Dfs())}");
            
            //show in unity
            positions =
                (mother.Start as DrawableVertex<DDorman>).Paint(new Vector3(0, 0, 0),
                    new List<DrawableVertex<DDorman>.ListElement>(),
                    dictionary);
        }

        public void RunOneRule()
        {
            grammar.ApplyOneRule();
            positions =
                (mother.Start as DrawableVertex<DDorman>).Paint(new Vector3(0, 0, 0),
                    new List<DrawableVertex<DDorman>.ListElement>(),
                    dictionary);
        }

        private void OnDrawGizmos()
        {
            Dictionary<DDorman.DDormanType, Color> colors = new Dictionary<DDorman.DDormanType, Color>();
            Dictionary<Vertex<DDorman>, Vector3> dict =
                new Dictionary<Vertex<DDorman>, Vector3>(
                    new IdentityEqualityComparer<Vertex<DDorman>>());

            foreach (DrawableVertex<DDorman>.ListElement position in positions)
            {
                Color color;
                if (colors.ContainsKey(position.t.Type.Type))
                {
                    color = colors[position.t.Type.Type];
                }
                else
                {
                    color = new Color(Random.value, Random.value, Random.value);
                    colors.Add(position.t.Type.Type, color);
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
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Start},
                new List<DDorman.DDormanType>()
                {
                    DDorman.DDormanType.Entrance, DDorman.DDormanType.Chain, DDorman.DDormanType.Gate,
                    DDorman.DDormanType.BossMini,
                    DDorman.DDormanType.ItemQuest, DDorman.DDormanType.TestItem,
                    DDorman.DDormanType.ChainFinal, DDorman.DDormanType.Goal
                });

            //rule non linear 00
            Graph<DDorman> left01 = CreateLinearGraph(new List<DDorman.DDormanType>()
                {DDorman.DDormanType.ChainFinal, DDorman.DDormanType.Goal});
            Graph<DDorman> right01 = new Graph<DDorman>();
            DrawableVertex<DDorman> c = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Chain));
            DrawableVertex<DDorman> h0 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Hook));
            DrawableVertex<DDorman> ga = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Gate));
            DrawableVertex<DDorman> lf = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.LockFinal));
            DrawableVertex<DDorman> bl = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.BossLevel));
            DrawableVertex<DDorman> go = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Goal));
            DrawableVertex<DDorman> t = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Test));
            DrawableVertex<DDorman> kf = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.KeyFinal));
            DrawableVertex<DDorman> h1 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Hook));
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
            GrammarRule<DDorman> rule = new GrammarRule<DDorman>(left01, right01);
            rules.Add(rule);

            //rule 01
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Chain, DDorman.DDormanType.Goal},
                new List<DDorman.DDormanType>()
                {
                    DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear,
                });

            //rule 02
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Chain, DDorman.DDormanType.Goal},
                new List<DDorman.DDormanType>()
                {
                    DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear,
                    DDorman.DDormanType.ChainLinear
                });

            //rule 03
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Chain, DDorman.DDormanType.Goal},
                new List<DDorman.DDormanType>()
                {
                    DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear,
                    DDorman.DDormanType.ChainLinear,
                    DDorman.DDormanType.ChainLinear
                });

            //rule 04
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.ChainLinear},
                new List<DDorman.DDormanType>() {DDorman.DDormanType.Test});

            //rule 05
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.ChainLinear},
                new List<DDorman.DDormanType>()
                    {DDorman.DDormanType.Test, DDorman.DDormanType.Test, DDorman.DDormanType.ItemBonus});

            //rule 06
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.ChainLinear},
                new List<DDorman.DDormanType>() {DDorman.DDormanType.TestSecret});

            //rule 07
            CreateLinearRule(
                new List<DDorman.DDormanType>() {DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear},
                new List<DDorman.DDormanType>() {DDorman.DDormanType.Key, DDorman.DDormanType.Lock});

            //rule 08
            CreateLinearRule(
                new List<DDorman.DDormanType>() {DDorman.DDormanType.ChainLinear, DDorman.DDormanType.ChainLinear},
                new List<DDorman.DDormanType>()
                    {DDorman.DDormanType.Key, DDorman.DDormanType.Lock, DDorman.DDormanType.ChainLinear});

            //rule 09
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Chain, DDorman.DDormanType.Gate},
                new List<DDorman.DDormanType>() {DDorman.DDormanType.ChainParallel, DDorman.DDormanType.Gate});

            //rule 10 
            CreateLinearRule(
                new List<DDorman.DDormanType>() {DDorman.DDormanType.Fork, DDorman.DDormanType.KeyMultiPiece},
                new List<DDorman.DDormanType>()
                    {DDorman.DDormanType.Fork, DDorman.DDormanType.Test, DDorman.DDormanType.KeyMultiPiece});

            //rule 11
            CreateLinearRule(
                new List<DDorman.DDormanType>() {DDorman.DDormanType.Fork, DDorman.DDormanType.KeyMultiPiece},
                new List<DDorman.DDormanType>()
                    {DDorman.DDormanType.Fork, DDorman.DDormanType.TestSecret, DDorman.DDormanType.KeyMultiPiece});

            //rule 12
            CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Fork, DDorman.DDormanType.Key},
                new List<DDorman.DDormanType>()
                    {DDorman.DDormanType.Fork, DDorman.DDormanType.TestSecret, DDorman.DDormanType.Key});

            // //hook resolve 01
            // CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Hook}, new List<DDorman.DDormanType>()
            // {
            //     DDorman.DDormanType.Nothing
            // });
            //
            // //hook resolve 02
            // CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Hook},
            //     new List<DDorman.DDormanType>() {DDorman.DDormanType.Test, DDorman.DDormanType.ItemBonus});
            //
            // //hook resolve 03
            // CreateLinearRule(new List<DDorman.DDormanType>() {DDorman.DDormanType.Hook},
            //     new List<DDorman.DDormanType>() {DDorman.DDormanType.TestSecret, DDorman.DDormanType.ItemBonus});

            //rule non-linear 01
            Graph<DDorman> left = CreateLinearGraph(new List<DDorman.DDormanType>()
                {DDorman.DDormanType.ChainParallel, DDorman.DDormanType.Gate});
            Graph<DDorman> right = new Graph<DDorman>();
            DrawableVertex<DDorman> f = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Fork));
            DrawableVertex<DDorman> km0 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.KeyMultiPiece));
            DrawableVertex<DDorman> km1 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.KeyMultiPiece));
            DrawableVertex<DDorman> km2 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.KeyMultiPiece));
            DrawableVertex<DDorman> lm = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.LockMulti));
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
            GrammarRule<DDorman> rule01 = new GrammarRule<DDorman>(left, right);
            rules.Add(rule01);

            //rule non-linear 02
            Graph<DDorman> left02 = CreateLinearGraph(new List<DDorman.DDormanType>()
                {DDorman.DDormanType.Fork, DDorman.DDormanType.KeyMultiPiece});
            Graph<DDorman> right02 = new Graph<DDorman>();
            DrawableVertex<DDorman> f0 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Fork));
            DrawableVertex<DDorman> k = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Key));
            DrawableVertex<DDorman> l = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Lock));
            DrawableVertex<DDorman> km3 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.KeyMultiPiece));
            DrawableVertex<DDorman> h = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Hook));
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
            GrammarRule<DDorman> rule02 = new GrammarRule<DDorman>(left02, right02);
            rules.Add(rule02);

            //rule non-linear 03
            Graph<DDorman> left03 = CreateLinearGraph(new List<DDorman.DDormanType>() {DDorman.DDormanType.Fork});
            Graph<DDorman> right03 = new Graph<DDorman>();
            DrawableVertex<DDorman> n = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Nothing));
            DrawableVertex<DDorman> h02 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Hook));
            DrawableVertex<DDorman> h03 = new DrawableVertex<DDorman>(new DDorman(DDorman.DDormanType.Hook));
            n.AddNextNeighbour(h02);
            n.AddNextNeighbour(h03);
            right03.AddVertex(n);
            right03.AddVertex(h02);
            right03.AddVertex(h03);
            right03.Start = n;
            right03.End = h02;
            GrammarRule<DDorman> rule03 = new GrammarRule<DDorman>(left03, right03);
            rules.Add(rule03);
        }

        private void CreateLinearRule(IList<DDorman.DDormanType> leftHandSide, IList<DDorman.DDormanType> rightHandSide)
        {
            Graph<DDorman> left = CreateLinearGraph(leftHandSide);
            Graph<DDorman> right = CreateLinearGraph(rightHandSide);
            GrammarRule<DDorman> rule = new GrammarRule<DDorman>(left, right);
            rules.Add(rule);
        }

        private Graph<DDorman> CreateLinearGraph(IList<DDorman.DDormanType> nodes)
        {
            Graph<DDorman> graph = new Graph<DDorman>();
            DrawableVertex<DDorman> prev = null;
            for (int index = 0; index < nodes.Count; index++)
            {
                DDorman.DDormanType type = nodes[index];
                DrawableVertex<DDorman> next = new DrawableVertex<DDorman>(new DDorman(type));
                graph.AddVertex(next);
                if (index == 0)
                {
                    graph.Start = next;
                }

                if (index == nodes.Count - 1)
                {
                    graph.End = next;
                }

                prev?.AddNextNeighbour(next);
                prev = next;
            }

            return graph;
        }
    }
}