using System;
using System.Collections.Generic;
using Framework.GraphGrammar;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

namespace Demo.GraphGrammar
{
    public class GraphGrammar1 : MonoBehaviour
    {
        public GameObject vertexRepresentation;

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

        private void Start()
        {
            //rule 01
            IList<DDorman> leftHandSide00 = new List<DDorman>() {DDorman.S};
            Graph<DDorman> left0 = CreateLinearGraph(leftHandSide00);
            
            IList<DDorman> rightHandSide00 = new List<DDorman>()
                {DDorman.E, DDorman.C, DDorman.Ga, DDorman.Bm, DDorman.Iq, DDorman.Ti, DDorman.Cf, DDorman.Go};
            Graph<DDorman> right0 = CreateLinearGraph(rightHandSide00);
            
            GrammarRule<DDorman> rule = new GrammarRule<DDorman>(left0, right0);

            Graph<DDorman> mother = new Graph<DDorman>();
            DrawableVertex<DDorman> m00 = new DrawableVertex<DDorman>(DDorman.S);
            mother.AddVertex(m00);
            mother.start = m00;
            mother.end = m00;

            //apply rule
            bool replaced = mother.ReplaceSubGraph(rule);
            Debug.Log($"Replaced: {replaced} {string.Join("; ", mother.Dfs())}");

            //show in unity
            List<DrawableVertex<DDorman>.ListElement> positions =
                (mother.start as DrawableVertex<DDorman>).Paint(new Vector3(0, 0, 0),
                    new List<DrawableVertex<DDorman>.ListElement>());

            foreach (DrawableVertex<DDorman>.ListElement position in positions)
            {
                Color color = Color.blue;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Renderer cubeRenderer = cube.GetComponent<Renderer>();
                cubeRenderer.material.SetColor("_Color", color);
                cube.transform.position = position.tPosition;
                cube.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                Debug.DrawLine(position.parent, position.tPosition, Color.blue, 10000);
            }
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