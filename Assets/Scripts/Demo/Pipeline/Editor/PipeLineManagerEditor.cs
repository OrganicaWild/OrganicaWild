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

            if (GUILayout.Button("Generate New"))
            {
                manager.seed = new System.Random().Next();
                manager.Setup();    
                manager.Generate();
            }

            if (GUILayout.Button("Regenerate"))
            {
                manager.Setup();
                manager.Generate();
            }

            manager.seed = EditorGUILayout.IntField("Seed", manager.seed, new GUILayoutOption[0]);

        }
    }
}