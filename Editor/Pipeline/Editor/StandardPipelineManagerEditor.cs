using System;
using UnityEditor;
using UnityEngine;

namespace Framework.Pipeline.Standard.Editor
{
    [CustomEditor(typeof(StandardPipelineManager))]
    public class StandardPipelineManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            StandardPipelineManager manager = (StandardPipelineManager) target;
            
            if (manager.HasError)
            {
                EditorGUILayout.HelpBox(manager.ErrorText, MessageType.Error);
                EditorGUILayout.HelpBox(manager.FixHelpText, MessageType.Info);
            }

            if (GUILayout.Button("Generate with new seed"))
            {
                manager.Seed = Environment.TickCount;
                Generate(manager);
            }

            if (GUILayout.Button("Generate with set seed"))
            {
                Generate(manager);
            }

            //manager.Seed = EditorGUILayout.IntField("Seed", manager.Seed);
            DrawDefaultInspector();
        }

        private void Generate(StandardPipelineManager manager)
        {
            manager.Setup();
            if (Application.isPlaying)
            {
                manager.StartCoroutine(manager.Generate());
            }
            else
            {
                manager.GenerateBlocking();
            }
         
        }

    
    }
}