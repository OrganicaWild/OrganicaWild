using UnityEditor;
using UnityEngine;

namespace Framework.GraphGrammar.Editor
{
    [CustomEditor(typeof(GraphGrammarComponent)), CanEditMultipleObjects]
    public class GraphGrammarComponentEditor : UnityEditor.Editor
    {
        private string[] types;
        
        public override void OnInspectorGUI()
        {
            GraphGrammarComponent grammarComponent = (GraphGrammarComponent) target;

            DrawDefaultInspector();
            //TypeControl(grammarComponent);

            if (GUILayout.Button("Reset Graph"))
            {
                grammarComponent.Initialize();
            }

            if (GUILayout.Button("One Rule"))
            {
                grammarComponent.ApplyOneRule();
            }

            if (GUILayout.Button("Run Until Termination"))
            {
                grammarComponent.Initialize();
                grammarComponent.ApplyUntilNoRulesFitAnymore();
            }
            
        }
    }
}