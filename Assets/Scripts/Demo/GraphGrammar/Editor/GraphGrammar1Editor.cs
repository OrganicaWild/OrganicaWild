using UnityEditor;
using UnityEngine;

namespace Demo.GraphGrammar
{
    [CustomEditor(typeof(DormanGrammar))]
    public class GraphGrammar1Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DormanGrammar grammar = (DormanGrammar) target;

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
                grammar.ApplyUntilFinished();
            }
        }
    }
}