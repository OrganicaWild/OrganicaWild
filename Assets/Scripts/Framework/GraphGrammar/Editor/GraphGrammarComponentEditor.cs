using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.GraphGrammar.Editor
{
    [CustomEditor(typeof(GraphGrammarComponent)), CanEditMultipleObjects]
    public class GraphGrammarComponentEditor : UnityEditor.Editor
    {
        private string newType = "";
        private string[] types;

        public override void OnInspectorGUI()
        {
            GraphGrammarComponent grammarComponent = (GraphGrammarComponent) target;
            
            //TypeControl(grammarComponent);

            if (GUILayout.Button("Reset Graph"))
            {
                grammarComponent.MakeGrammar();
            }

            if (GUILayout.Button("One Rule"))
            {
                grammarComponent.RunOneRule();
            }

            if (GUILayout.Button("Run Until Termination"))
            {
                grammarComponent.MakeGrammar();
                grammarComponent.ApplyUntilFinished();
            }
        }

        private void TypeControl(GraphGrammarComponent grammarComponent)
        {
            GUIStyle style = new GUIStyle
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = {textColor = Color.white}
            };

            GUILayout.Label("All Types", style);
            
            types = grammarComponent.types.ToArray();
            string allTypes = types.Aggregate("", (current, type) => current + ($"{type} \n"));
            GUILayout.Label(allTypes);

            newType = EditorGUILayout.TextField("New Type", newType);

            if (GUILayout.Button("Add new Type"))
            {
                if (newType != "")
                {
                    grammarComponent.types.Add(newType);
                    newType = "";
                }
            }
        }
    }
}