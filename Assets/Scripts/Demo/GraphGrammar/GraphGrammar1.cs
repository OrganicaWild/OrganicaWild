using System;
using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using Framework.Util;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using Random = UnityEngine.Random;

namespace Demo.GraphGrammar
{
    public class GraphGrammar1 : MonoBehaviour
    {
        public GameObject vertexRepresentation;

        [Serializable]
        private enum DDorman
        {
            BL, //boss lever
            Bm, //mini boss
            C, //chain
            Cf, //chain final
            Cl, //chain linear
            Cp, //chain parallel
            E, //entrance
            F, //fork
            Ga, //gate
            Go, //goal
            H, //hook
            Ib, //item bonus
            Iq, //item quest
            K, //key
            Kf, //key (final)
            Km, //key multi piece
            L, //lock
            Lf, //lock (final)
            Lm, //lock (multi)
            N, //nothing, exploration
            S, //Start
            T, //test
            Ti, //test (item)
            Ts //test (secret)
        }

        private IList<GrammarRule<DDorman>> rules = new List<GrammarRule<DDorman>>();

        private void Start()
        {
            BuildRules();

            //mother graph
            Graph<DDorman> mother = CreateLinearGraph(new List<DDorman>() {DDorman.S});

            //apply rule
            bool replaced = mother.ReplaceSubGraph(rules[0]);
            replaced = mother.ReplaceSubGraph(rules[1]);
            Debug.Log($"Replaced: {replaced} {string.Join("; ", mother.Dfs())}");

            //show in unity
            List<DrawableVertex<DDorman>.ListElement> positions =
                (mother.start as DrawableVertex<DDorman>).Paint(new Vector3(0, 0, 0),
                    new List<DrawableVertex<DDorman>.ListElement>(),
                    new Dictionary<Vertex<DDorman>, Vector3>(new IdentityEqualityComparer<Vertex<DDorman>>()));

            DrawGraph(positions);
        }

        private static void DrawGraph(List<DrawableVertex<DDorman>.ListElement> positions)
        {
            Dictionary<DDorman, Color> colors = new Dictionary<DDorman, Color>();
            Dictionary<Vertex<DDorman>, Vector3> dict =
                new Dictionary<Vertex<DDorman>, Vector3>(new IdentityEqualityComparer<Vertex<DDorman>>());

            foreach (DrawableVertex<DDorman>.ListElement position in positions)
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
                    Debug.DrawLine(position.parent, dict[position.t], Color.blue, 10000);
                }
                else
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = position.t.Type.ToString();
                    Renderer cubeRenderer = cube.GetComponent<Renderer>();
                    cubeRenderer.material.SetColor("_Color", color);
                    cube.transform.position = position.tPosition;
                    cube.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                    Debug.DrawLine(position.parent, position.tPosition, Color.blue, 10000);
                    dict.Add(position.t, position.tPosition);
                }
            }
        }

        private void BuildRules()
        {
            //rule 00
            CreateLinearRule(new List<DDorman>() {DDorman.S},
                new List<DDorman>()
                    {DDorman.E, DDorman.C, DDorman.Ga, DDorman.Bm, DDorman.Iq, DDorman.Ti, DDorman.Cf, DDorman.Go});

            //rule non linear 00
            Graph<DDorman> left01 = CreateLinearGraph(new List<DDorman>() {DDorman.Cf, DDorman.Go});
            Graph<DDorman> right01 = new Graph<DDorman>();
            DrawableVertex<DDorman> c = new DrawableVertex<DDorman>(DDorman.C);
            DrawableVertex<DDorman> h0 = new DrawableVertex<DDorman>(DDorman.H);
            DrawableVertex<DDorman> ga = new DrawableVertex<DDorman>(DDorman.Ga);
            DrawableVertex<DDorman> lf = new DrawableVertex<DDorman>(DDorman.Lf);
            DrawableVertex<DDorman> bl = new DrawableVertex<DDorman>(DDorman.BL);
            DrawableVertex<DDorman> go = new DrawableVertex<DDorman>(DDorman.Go);
            DrawableVertex<DDorman> t = new DrawableVertex<DDorman>(DDorman.T);
            DrawableVertex<DDorman> kf = new DrawableVertex<DDorman>(DDorman.Kf);
            DrawableVertex<DDorman> h1 = new DrawableVertex<DDorman>(DDorman.H);
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
            right01.start = c;
            right01.end = go;
            GrammarRule<DDorman> rule = new GrammarRule<DDorman>(left01, right01);
            rules.Add(rule);

            //rule 01
            CreateLinearRule(new List<DDorman>() {DDorman.C, DDorman.Go},
                new List<DDorman>() {DDorman.Cl, DDorman.Cl, DDorman.Cl,});

            //rule 02
            CreateLinearRule(new List<DDorman>() {DDorman.C, DDorman.Go},
                new List<DDorman>() {DDorman.Cl, DDorman.Cl, DDorman.Cl, DDorman.Cl});

            //rule 03
            CreateLinearRule(new List<DDorman>() {DDorman.C, DDorman.Go},
                new List<DDorman>() {DDorman.Cl, DDorman.Cl, DDorman.Cl, DDorman.Cl, DDorman.Cl});

            //rule 04
            CreateLinearRule(new List<DDorman>() {DDorman.Cl},
                new List<DDorman>() {DDorman.T});

            //rule 05
            CreateLinearRule(new List<DDorman>() {DDorman.Cl},
                new List<DDorman>() {DDorman.T, DDorman.T, DDorman.Ib});

            //rule 06
            CreateLinearRule(new List<DDorman>() {DDorman.Cl},
                new List<DDorman>() {DDorman.Ts});

            //rule 07
            CreateLinearRule(new List<DDorman>() {DDorman.Cl, DDorman.Cl},
                new List<DDorman>() {DDorman.K, DDorman.L});

            //rule 08
            CreateLinearRule(new List<DDorman>() {DDorman.Cl, DDorman.Cl},
                new List<DDorman>() {DDorman.K, DDorman.L, DDorman.Cl});

            //rule 09
            CreateLinearRule(new List<DDorman>() {DDorman.C, DDorman.Ga},
                new List<DDorman>() {DDorman.Cp, DDorman.Ga});

            //rule 10 
            CreateLinearRule(new List<DDorman>() {DDorman.F, DDorman.Km},
                new List<DDorman>() {DDorman.F, DDorman.T, DDorman.Km});

            //rule 11
            CreateLinearRule(new List<DDorman>() {DDorman.F, DDorman.Km},
                new List<DDorman>() {DDorman.F, DDorman.Ts, DDorman.Km});

            //rule 12
            CreateLinearRule(new List<DDorman>() {DDorman.F, DDorman.K},
                new List<DDorman>() {DDorman.F, DDorman.Ts, DDorman.K});

            //rule non-linear 01
            Graph<DDorman> left = CreateLinearGraph(new List<DDorman>() {DDorman.Cp, DDorman.Ga});
            Graph<DDorman> right = new Graph<DDorman>();
            DrawableVertex<DDorman> f = new DrawableVertex<DDorman>(DDorman.F);
            DrawableVertex<DDorman> km0 = new DrawableVertex<DDorman>(DDorman.Km);
            DrawableVertex<DDorman> km1 = new DrawableVertex<DDorman>(DDorman.Km);
            DrawableVertex<DDorman> km2 = new DrawableVertex<DDorman>(DDorman.Km);
            DrawableVertex<DDorman> lm = new DrawableVertex<DDorman>(DDorman.Lm);
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
            right.start = f;
            right.end = lm;
            GrammarRule<DDorman> rule01 = new GrammarRule<DDorman>(left, right);
            rules.Add(rule01);

            //rule non-linear 02
            Graph<DDorman> left02 = CreateLinearGraph(new List<DDorman>() {DDorman.F, DDorman.Km});
            Graph<DDorman> right02 = new Graph<DDorman>();
            DrawableVertex<DDorman> f0 = new DrawableVertex<DDorman>(DDorman.F);
            DrawableVertex<DDorman> k = new DrawableVertex<DDorman>(DDorman.K);
            DrawableVertex<DDorman> l = new DrawableVertex<DDorman>(DDorman.L);
            DrawableVertex<DDorman> km3 = new DrawableVertex<DDorman>(DDorman.Km);
            DrawableVertex<DDorman> h = new DrawableVertex<DDorman>(DDorman.H);
            f0.AddNextNeighbour(k);
            k.AddNextNeighbour(l);
            l.AddNextNeighbour(km3);
            l.AddNextNeighbour(h);
            right02.AddVertex(f0);
            right02.AddVertex(k);
            right02.AddVertex(l);
            right02.AddVertex(km3);
            right02.AddVertex(h);
            right02.start = f0;
            right02.end = h;
            GrammarRule<DDorman> rule02 = new GrammarRule<DDorman>(left02, right02);
            rules.Add(rule02);

            //rule non-linear 03
            Graph<DDorman> left03 = CreateLinearGraph(new List<DDorman>() {DDorman.F});
            Graph<DDorman> right03 = new Graph<DDorman>();
            DrawableVertex<DDorman> n = new DrawableVertex<DDorman>(DDorman.N);
            DrawableVertex<DDorman> h02 = new DrawableVertex<DDorman>(DDorman.H);
            DrawableVertex<DDorman> h03 = new DrawableVertex<DDorman>(DDorman.H);
            n.AddNextNeighbour(h02);
            n.AddNextNeighbour(h03);
            right03.AddVertex(n);
            right03.AddVertex(h02);
            right03.AddVertex(h03);
            right03.start = n;
            right03.end = h02;
            GrammarRule<DDorman> rule03 = new GrammarRule<DDorman>(left03, right03);
            rules.Add(rule03);
        }

        private void CreateLinearRule(IList<DDorman> leftHandSide, IList<DDorman> rightHandSide)
        {
            Graph<DDorman> left = CreateLinearGraph(leftHandSide);
            Graph<DDorman> right = CreateLinearGraph(rightHandSide);
            GrammarRule<DDorman> rule = new GrammarRule<DDorman>(left, right);
            rules.Add(rule);
        }

        private Graph<DDorman> CreateLinearGraph(IList<DDorman> nodes)
        {
            Graph<DDorman> graph = new Graph<DDorman>();
            DrawableVertex<DDorman> prev = null;
            for (int index = 0; index < nodes.Count; index++)
            {
                DDorman type = nodes[index];
                DrawableVertex<DDorman> next = new DrawableVertex<DDorman>(type);
                graph.AddVertex(next);
                if (index == 0)
                {
                    graph.start = next;
                }

                if (index == nodes.Count - 1)
                {
                    graph.end = next;
                }

                prev?.AddNextNeighbour(next);
                prev = next;
            }

            return graph;
        }
    }
}