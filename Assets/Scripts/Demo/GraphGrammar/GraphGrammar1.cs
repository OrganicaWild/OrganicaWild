using System;
using Framework.GraphGrammar;
using UnityEngine;

namespace Demo.GraphGrammar
{
    public class GraphGrammar1 : MonoBehaviour
    {
        private void Start()
        {
            Graph left0 = new Graph();
            Vertex left01 = new Vertex(0);
            left0.AddVertex(left01);
            left0.start = left01;
            left0.end = left01;
            
            Graph right0 = new Graph();
            Vertex right01 = new Vertex(2);
            Vertex right02 = new Vertex(3);
            right0.AddVertex(right01);
            right0.AddVertex(right02);
            right01.AddNextNeighbour(right02);
            right0.start = right01;
            right0.end = right02;

            GrammarRule rule = new GrammarRule(left0, right0);
            
            Graph mother = new Graph();
            Vertex m00 = new Vertex(0);
            Vertex m01 = new Vertex(0);
            Vertex m02 = new Vertex(1);
            Vertex m03 = new Vertex(1);
            mother.AddVertex(m00);
            mother.AddVertex(m01);
            mother.AddVertex(m02);
            mother.AddVertex(m03);
            m00.AddNextNeighbour(m01);
            m01.AddNextNeighbour(m02);
            m02.AddNextNeighbour(m03);
            mother.start = m00;
            mother.end = m03;

            // bool contains = mother.Contains(rule.LeftHandSide);
            // Debug.Log(contains);
            bool replaced = mother.ReplaceSubGraph(rule);
            Debug.Log($"Replaced: {replaced} {String.Join("; ", mother.Dfs())}");
        }
    }
}