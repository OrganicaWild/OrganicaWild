using UnityEditor;
using UnityEngine;

namespace Demo.GraphGrammar
{
    [CustomEditor(typeof(GraphGrammar1))]
    public class GraphGrammar1Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            GraphGrammar1 grammar = (GraphGrammar1) target;

            if (GUILayout.Button("Reset Graph"))
            {
                grammar.MakeGrammar();
            }
            
            if (GUILayout.Button("One Rule"))
            {
                grammar.RunOneRule();
            }
            
            if (GUILayout.Button("Run Until Termination"))
            {
                grammar.MakeGrammar();
                grammar.grammar.ApplyUntilNoNonTerminal();
            }
        }
    }
}