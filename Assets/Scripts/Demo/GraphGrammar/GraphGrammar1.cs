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
        
        private void Start()
        {
            //build graph
            Graph left0 = new Graph();
            DrawableVertex left01 = new DrawableVertex(0);
            left0.AddVertex(left01);
            left0.start = left01;
            left0.end = left01;

            Graph right0 = new Graph();
            DrawableVertex right01 = new DrawableVertex(2);
            DrawableVertex right02 = new DrawableVertex(3);
            right0.AddVertex(right01);
            right0.AddVertex(right02);
            right01.AddNextNeighbour(right02);
            right0.start = right01;
            right0.end = right02;

            GrammarRule rule = new GrammarRule(left0, right0);

            Graph mother = new Graph();
            DrawableVertex m00 = new DrawableVertex(0);
            DrawableVertex m01 = new DrawableVertex(0);
            DrawableVertex m02 = new DrawableVertex(1);
            DrawableVertex m03 = new DrawableVertex(1);
            mother.AddVertex(m00);
            mother.AddVertex(m01);
            mother.AddVertex(m02);
            mother.AddVertex(m03);
            m00.AddNextNeighbour(m01);
            m01.AddNextNeighbour(m02);
            m02.AddNextNeighbour(m03);
            mother.start = m00;
            mother.end = m03;

            //apply rule
            bool replaced = mother.ReplaceSubGraph(rule);
            Debug.Log($"Replaced: {replaced} {String.Join("; ", mother.Dfs())}");

            //show in unity
            List<DrawableVertex.ListElement> positions =
                (mother.start as DrawableVertex).Paint(new Vector3(0, 0, 0), new List<DrawableVertex.ListElement>());
            
            foreach (DrawableVertex.ListElement position in positions)
            {
                Instantiate(vertexRepresentation, position.tPosition, Quaternion.identity);
                Debug.DrawLine(position.parent, position.tPosition, Color.blue, 10000);
            }
        }
    }
}