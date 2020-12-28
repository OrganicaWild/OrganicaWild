using Framework.GraphGrammar.Data;
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
            if (GUILayout.Button("Edit"))
            {
                GraphEditor window = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor), false, $"{target}");
                window.Setup(target as EditorMissionGraph);
                window.Show();
            }
        }
    }
}