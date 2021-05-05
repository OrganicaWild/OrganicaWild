using UnityEditor;
using UnityEngine;

namespace Demo.Pipeline.Editor
{
    [CustomEditor(typeof(PipelineManager))]
    public class PipeLineManagerEditor : UnityEditor.Editor
    {
        public float MinDebugColorBrightness { get; set; } = 0.7f;

        public override void OnInspectorGUI()
        {
            PipelineManager manager = (PipelineManager) target;
            
            //DrawDefaultInspector();

            if (manager.hasError)
            {
                EditorGUILayout.HelpBox(manager.errorText, MessageType.Error);
            }

            if (GUILayout.Button("Generate with new seed"))
            {
                manager.seed = new System.Random().Next();
                Generate(manager);
            }

            if (GUILayout.Button("Generate with set seed"))
            {
                Generate(manager);
            }

            manager.seed = EditorGUILayout.IntField("Seed", manager.seed);
            manager.startOnStartup = EditorGUILayout.Toggle("Regenerate On Startup",manager.startOnStartup);

        }

        private void Generate(PipelineManager manager)
        {
            manager.Setup();
            manager.Generate();
        }
    }
}