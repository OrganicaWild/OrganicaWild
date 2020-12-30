using Framework.GraphGrammar.EditorData;
using UnityEditor;
using UnityEngine;

namespace Framework.GraphGrammar.Editor
{
    [CustomEditor(typeof(EditorMissionGraph))]
    public class MissionGraphEditor : UnityEditor.Editor
    {
        private void Start()
        {
            base.OnInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            EditorMissionGraph graph = target as EditorMissionGraph;

            if (!(graph is null))
            {
                
                if (GUILayout.Button("Edit"))
                {
                    GraphEditor window = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor), false, $"{target}");
                    window.Setup(graph);
                    window.Show();
                }
                GUILayout.Label(graph.serializedMissionGraph);
                
            }
        }
    }
}